using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class VRPlayer : NetworkBehaviour
{

	public Camera childCamera;
	void OnStartLocalPlayer()
	{
		//test code
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
			gameObject.AddComponent<ARPlayer>();
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
