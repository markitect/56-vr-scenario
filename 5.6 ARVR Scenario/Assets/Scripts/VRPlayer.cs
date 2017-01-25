using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;


public class VRPlayer : NetworkBehaviour
{
    public GameObject ARPlayerInstance;
	public GameObject VRPlayerInstance;
	public Camera childCamera;
    public Camera arChildCamera;
	public NetworkTransformChild netWorkTransfromChild;

    GameObject LaserBlock1;
    GameObject LaserBlock2;
    GameObject RedMirror;

	void OnStartLocalPlayer()
	{
		//test code
	}

	void Start()
	{
        //LaserBlock1 = GameObject.Find("LaserBlock1");
        //LaserBlock2 = GameObject.Find("LaserBlock2");
        //RedMirror = GameObject.Find("RedMirror");

        if (!isLocalPlayer)
		{
			Destroy(childCamera);
			return;
		}

		if (VRSettings.loadedDeviceName == "HoloLens")
		{
            RpcActivateArRig();
            gameObject.AddComponent<ArControls>();
        }
	}

    [ClientRpc]
    public void RpcActivateArRig()
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
