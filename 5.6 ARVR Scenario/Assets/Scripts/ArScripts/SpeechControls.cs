using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;

public class SpeechControls : MonoBehaviour
{

    KeywordRecognizer m_KeywordRecognizer = null;
    GestureRecognizer m_GestureReconizer = null;
    public string[] controlWords;
    public GameObject Block1;
    public GameObject Block2;

    bool Block1Moved = false;
    bool Block2Moved = false;
    bool wallTracking = false;
    private float m_currentTrackSpeed = 6.5f;

    void Start()
    {
        m_KeywordRecognizer = new KeywordRecognizer(controlWords);
        m_KeywordRecognizer.OnPhraseRecognized += M_KeywordRecognizer_OnPhraseRecognized;
        m_KeywordRecognizer.Start();

        m_GestureReconizer = new GestureRecognizer();
        m_GestureReconizer.SetRecognizableGestures(GestureSettings.Tap
            | GestureSettings.NavigationX
            | GestureSettings.NavigationY
            | GestureSettings.NavigationZ);
        m_GestureReconizer.TappedEvent += M_GestureReconizer_TappedEvent;
        m_GestureReconizer.StartCapturingGestures();
    }

    void Update()
    {
        var cam = Camera.main.transform;
        if (wallTracking)
        {
            if (Block1Moved)
            {
                var wall = GameObject.Find("LaserBlock(1)");
                Vector3 move = cam.forward * 4f + cam.position;
                wall.transform.position = Vector3.Lerp(wall.transform.position, move, Time.deltaTime * m_currentTrackSpeed);
                wall.transform.rotation = Quaternion.Lerp(wall.transform.rotation, cam.rotation, Time.deltaTime * m_currentTrackSpeed);
            }

            if (Block2Moved)
            {
                var wall = GameObject.Find("LaserBlock(2)");
                Vector3 move = cam.forward * 4f + cam.position;
                wall.transform.position = Vector3.Lerp(wall.transform.position, move, Time.deltaTime * m_currentTrackSpeed);
                wall.transform.rotation = Quaternion.Lerp(wall.transform.rotation, cam.rotation, Time.deltaTime * m_currentTrackSpeed);
            }
        }
    }
    
    private void M_GestureReconizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (!wallTracking)
        {
            wallTracking = true;
        }
        else if (wallTracking)
        {
            wallTracking = false;
        }
    }

    private void M_KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text == "One")
        {
            Block1Moved = true;
            Block2Moved = false;
        }

        if(args.text == "Two")
        {
            Block1Moved = false;
            Block2Moved = true;
        }
    }

    public void SpawnWall()
    {
        RaycastHit hit;
        var cam = Camera.main.transform;

        if (Physics.Raycast(cam.position, Vector3.forward, out hit, 30f))
        {
            //Block1 = Instantiate(LaserWall);
            wallTracking = true;
        }
    }
}
