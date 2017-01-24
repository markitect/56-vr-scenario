using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ARPlayer : NetworkBehaviour {

	//temporary test location for HoloLens
	Vector3 testLocation = new Vector3(2.77f,7.69f,-10.04f);

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
		{
			return;
		}
		transform.localPosition = testLocation;
	}

	void OnStartLocalPlayer()
	{
		//test code
		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
