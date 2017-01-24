using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class VRPlayer : NetworkBehaviour
{
    public GameObject ArPlayer;
	public Camera childCamera;
    GameObject LaserBlock1;
    GameObject LaserBlock2;
    GameObject RedMirror;

	void OnStartLocalPlayer()
	{
		//test code
	}

	void Start()
	{
        LaserBlock1 = GameObject.Find("LaserBlock1");
        LaserBlock2 = GameObject.Find("LaserBlock2");
        RedMirror = GameObject.Find("RedMirror");

        if (!isLocalPlayer)
		{
			Destroy(childCamera);
			return;
		}

		if (VRSettings.loadedDeviceName == "HoloLens")
		{
            // We to destroy VR camera to create a AR camera
            Destroy(childCamera);

            GameObject ARInstance = Instantiate(ArPlayer);       
            ARInstance.transform.SetParent(transform);
            var child = transform.FindChild("VRPlayer");
            transform.FindChild("VRPlayer").SetParent(ARInstance.transform);
            child.localPosition = new Vector3(0f, 0f, 0f);
            NetworkServer.Spawn(ARInstance);

            //LaserBlock1.SetActive(true);
            //LaserBlock2.SetActive(true);
            //RedMirror.SetActive(true);
		}
	}


	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
	}
}
