using UnityEngine;
#if UNITY_WSA || UNITY_EDITOR
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;
#endif
using UnityEngine.Networking;

public class ArControls : NetworkBehaviour
{
#if UNITY_WSA || UNITY_EDITOR
    KeywordRecognizer keywordRecognizer = null;
    GestureRecognizer gestureReconizer = null;
    GameObject laserWallPrefab;
    GameObject redWindowPrefab;
    GameObject blueWindowPrefab;
    GameObject yellowWindowPrefab;
    GameObject localWall1;
    GameObject localWall2;
#endif

    bool redWindowTracking = false;
    bool blueWindowTracking = false;
    bool yellowWindowTracking = false;
    bool laserBlockTracking = true;

    string[] controlWords;
    private float currentTrackSpeed = 6.5f;
    float wallCoolDown1 = 0f;
    float wallCoolDown2 = 0f;
    static float coolDown = 10f;
    int wallCount = 0;
    int allowedWalls = 2;

	public GameObject arCamera;
	void Start()
	{
		if (!isLocalPlayer)
			arCamera.SetActive(false);
#if UNITY_WSA || UNITY_EDITOR
        controlWords = new string[] { "Wall" , "Red", "Blue", "Yellow" };

        keywordRecognizer = new KeywordRecognizer(controlWords);
        keywordRecognizer.OnPhraseRecognized += M_KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        gestureReconizer = new GestureRecognizer();
        gestureReconizer.SetRecognizableGestures(GestureSettings.Tap
            | GestureSettings.NavigationX
            | GestureSettings.NavigationY
            | GestureSettings.NavigationZ);
        gestureReconizer.TappedEvent += M_GestureReconizer_TappedEvent;
        gestureReconizer.NavigationUpdatedEvent += M_GestureReconizer_NavigationUpdatedEvent;
        gestureReconizer.StartCapturingGestures();
#endif

        laserWallPrefab = Resources.Load("ArResources/Prefabs/LaserBlock") as GameObject;
        redWindowPrefab = Resources.Load("ArResources/Prefabs/RedWindow") as GameObject;
        blueWindowPrefab = Resources.Load("ArResources/Prefabs/BlueWindow") as GameObject;
        yellowWindowPrefab = Resources.Load("ArResources/Prefabs/YellowWindow") as GameObject;
    }

		

    void Update()
    {
        UpdateLoopForWalls();
        UpdateLoopForWindows();
    }

#if UNITY_WSA || UNITY_EDITOR
    private void M_GestureReconizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (laserBlockTracking)
        {
            laserBlockTracking = false;
        }

        if(redWindowTracking)
        {
            redWindowTracking = false;
        }

        if (blueWindowTracking)
        {
            blueWindowTracking = false;
        }

        if (yellowWindowTracking)
        {
            yellowWindowTracking = false;
        }
    }

    private void M_KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text == "Wall")
        {
            if (wallCount < allowedWalls)
            {
                laserBlockTracking = true;
                wallCount += 1;

                if (isLocalPlayer)
                {
                    CmdSpawnWallObject();
                }

            }
            else if (wallCount > allowedWalls)
            {
                laserBlockTracking = false;
                Debug.Log("Can't Spawn Any Walls");
            }
        }

        if (args.text == "Red")
        {
            redWindowTracking = true;
            if(isLocalPlayer)
            {
                CmdSpawnRedWindowObject();
            }
        }

        if (args.text == "Blue")
        {
            blueWindowTracking = true;
            if (isLocalPlayer)
            {
                CmdSpawnBlueWindowObject();
            }
        }

        if (args.text == "Yellow")
        {
            yellowWindowTracking = true;
            if (isLocalPlayer)
            {
                CmdSpawnYellowWindowObject();
            }
        }
    }

    private void M_GestureReconizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(headRay, out hit, 50f))
        {
            if (hit.transform.gameObject.layer == 9)
            {
                var hitObject = hit.transform.gameObject;
                hitObject.transform.Rotate(normalizedOffset);
            }

            if (hit.transform.gameObject.layer == 10)
            {
                var hitObject = hit.transform.gameObject;
                hitObject.transform.Rotate(normalizedOffset);
            }
        }
    }
#endif

    public void MoveWall()
    {
        if (wallCount.Equals(1))
        {
            if (localWall1 == null)
            {
                return;
            }
            MoveObject(localWall1);
        }

        if (wallCount.Equals(2))
        {
            if (localWall1 == null)
            {
                return;
            }
            MoveObject(localWall2);
        }
    }

    public void UpdateLoopForWalls()
    {
        if (laserBlockTracking)
        {
            MoveWall();
        }

        if (wallCoolDown1 < 0)
        {
            if (localWall1)
            {
                Destroy(localWall1);
                localWall1 = null;
                wallCount -= 1;
            }
        }

        if (wallCoolDown2 < 0)
        {
            if (localWall2)
            {
                Destroy(localWall2);
                localWall2 = null;
                wallCount -= 1;
            }
        }

        if (wallCoolDown1 > 0 | wallCoolDown2 > 0)
        {
            wallCoolDown1 -= Time.deltaTime;
            wallCoolDown2 -= Time.deltaTime;
        }
    }

    public void UpdateLoopForWindows()
    {
        if (redWindowTracking)
        {
            MoveObject(redWindowPrefab);
        }

        if (blueWindowTracking)
        {
            MoveObject(blueWindowPrefab);
        }

        if (yellowWindowTracking)
        {
            MoveObject(yellowWindowPrefab);
        }
    }

    public void MoveObject(GameObject obj)
    {
        var cam = transform.GetComponentInChildren<Camera>().transform;
        Vector3 move = cam.forward * 4f + cam.position;
        obj.transform.position = Vector3.Lerp(obj.transform.position, move, Time.deltaTime * currentTrackSpeed);
        obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, cam.rotation, Time.deltaTime * currentTrackSpeed);
    }

    [Command]
    public void CmdSpawnWallObject()
    {
        if (!localWall1)
        {
            localWall1 = Instantiate(laserWallPrefab);
            NetworkServer.Spawn(localWall1);
            wallCoolDown1 = coolDown;
        }
        else if(localWall1)
        {
            localWall2 = Instantiate(laserWallPrefab);
            NetworkServer.Spawn(localWall2);
            wallCoolDown2 = coolDown;
        }
    }

    [Command]
    public void CmdSpawnRedWindowObject()
    {
        var window = Instantiate(redWindowPrefab);
        NetworkServer.Spawn(window);
        wallCoolDown1 = coolDown;
    }

    [Command]
    public void CmdSpawnYellowWindowObject()
    {
        var window = Instantiate(yellowWindowPrefab);
        NetworkServer.Spawn(window);
        wallCoolDown1 = coolDown;
    }

    [Command]
    public void CmdSpawnBlueWindowObject()
    {
        var window = Instantiate(blueWindowPrefab);
        NetworkServer.Spawn(window);
        wallCoolDown1 = coolDown;
    }
}

