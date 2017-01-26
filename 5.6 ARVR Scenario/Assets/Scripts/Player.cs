using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;


public class Player : NetworkBehaviour
{

    public GameObject ARPlayerInstance;
    public GameObject VRPlayerInstance;


	public Camera childCamera;
    public PlayerRoles role { get; private set; }

    public Camera arChildCamera;
    public NetworkTransformChild netWorkTransfromChild;

	void OnStartLocalPlayer()
	{
    }

	void Start()
	{
        if (!isLocalPlayer)
		{
			Destroy(childCamera);
			return;
		}



        if (VRSettings.loadedDeviceName == "HoloLens")
        {
            CmdActivateArRig();
            role = PlayerRoles.Blocker;
            gameObject.AddComponent<ArControls>();
        }

        else
        {
            role = PlayerRoles.Shooter;
            gameObject.AddComponent<ToolController>();
        }
    }



	[Command]
    public void CmdActivateArRig()
    {
        // We to destroy VR camera to create a AR camera
        Destroy(childCamera);

        ARPlayerInstance.SetActive(true);
        VRPlayerInstance.SetActive(false);
        netWorkTransfromChild.target = ARPlayerInstance.transform;
    }

    void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
	}
}
