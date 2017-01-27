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
	}

    void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
	}
}
