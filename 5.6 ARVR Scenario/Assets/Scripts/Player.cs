using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;


public class Player : NetworkBehaviour
{

    public GameObject ARPlayerInstance;
    public GameObject VRPlayerInstance;
	public GameObject VRPlayerTools;


	public Camera childCamera;
    public PlayerRoles role { get; private set; }

    public Camera arChildCamera;
    public NetworkTransformChild netWorkTransfromChild;

	void OnStartLocalPlayer()
	{

    }

	void Start()
	{
        if (VRSettings.loadedDeviceName == "HoloLens")
        {
            CmdActivateArRig();
            //RpcActivateArRig();
            role = PlayerRoles.Blocker;
            gameObject.AddComponent<ArControls>();
        }
        else
        {
            role = PlayerRoles.Shooter;
        }

		if (!isLocalPlayer)
		{
			Destroy(childCamera);
			return;
		}
	}



	[Command]
    public void CmdActivateArRig()
    {
        // We to destroy VR camera to create a AR camera
        Destroy(childCamera);

		gameObject.GetComponent<ToolController>().enabled = false;

		ARPlayerInstance.SetActive(true);
        VRPlayerInstance.SetActive(false);
		VRPlayerTools.SetActive(false);
		netWorkTransfromChild.target = ARPlayerInstance.transform;
    }

    [ClientRpc]
    public void RpcActivateArRig()
    {
        // We to destroy VR camera to create a AR camera
        Destroy(childCamera);

        gameObject.GetComponent<ToolController>().enabled = false;

        ARPlayerInstance.SetActive(true);
        VRPlayerInstance.SetActive(false);
        VRPlayerTools.SetActive(false);
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
