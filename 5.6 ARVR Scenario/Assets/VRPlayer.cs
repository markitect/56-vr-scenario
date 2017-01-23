using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class VRPlayer : NetworkBehaviour {
	void OnStartLocalPlayer()
	{
		GetComponent<Material>().color = Color.blue;

		//test code
		transform.position = new Vector3(Random.Range(-2,2), 1, Random.Range(-2, 2));
		VRSettings.LoadDeviceByName("OpenVR");
	}


	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
	}
}
