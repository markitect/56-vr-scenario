using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;


public class Player : NetworkBehaviour
{
    public GameObject VRPlayerInstance;
	public GameObject VRPlayerTools;

	public Camera childCamera;

    public Camera arChildCamera;
    public NetworkTransformChild netWorkTransfromChild;

	void Start()
	{
		if (!isLocalPlayer)
		{
			Destroy(childCamera);
			return;
		}

		this.gameObject.name = "Me";
	}

    void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
	}
}
