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
		GetComponent<Material>().color = Color.blue;
		//test code
	}

	void Start()
	{
		if (!isLocalPlayer)
		{
			Destroy(childCamera);
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
