﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;


public class VRPlayer : NetworkBehaviour
{
    [SerializeField] private Transform m_ToolPosition;
    [SerializeField] private GameObject[] m_AvailableTools;
    public GameObject ARPlayerInstance;
    public GameObject VRPlayerInstance;
    [SerializeField] private float m_ChangeTimeLimit;

    private GameObject[] m_ActiveTools;
    private int m_CurrentToolIndex;
    private float m_ToolChangeTimer;
    private bool b_CanChangeTool;
	public Camera childCamera;
    public PlayerRoles role { get; private set; }

    public Camera arChildCamera;
    public NetworkTransformChild netWorkTransfromChild;

    GameObject LaserBlock1;
    GameObject LaserBlock2;
    GameObject RedMirror;

	void OnStartLocalPlayer()
	{
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

        b_CanChangeTool = true;

        if (VRSettings.loadedDeviceName == "HoloLens")
        {
            RpcActivateArRig();
            role = PlayerRoles.Blocker;
            gameObject.AddComponent<ArControls>();
        }
        else
        {
            role = PlayerRoles.Shooter;

            m_ActiveTools = new GameObject[m_AvailableTools.Length];

            for (int i = 0; i < m_AvailableTools.Length; i++)
            {
                m_ActiveTools[i] = Instantiate(m_AvailableTools[i], m_ToolPosition, false);
            }

            m_ActiveTools[0].SetActive(true);
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

        Debug.Log("Horizontal: " + Input.GetAxis("Horizontal"));

	    m_ToolChangeTimer += Time.deltaTime;

	    if (Input.GetAxis("Horizontal") > .5f && b_CanChangeTool)
	    {
            m_ActiveTools[m_CurrentToolIndex].SetActive(false);
            m_CurrentToolIndex = (m_CurrentToolIndex + 1) % m_ActiveTools.Length;
            m_ActiveTools[m_CurrentToolIndex].SetActive(true);

	        m_ToolChangeTimer = 0;
	        b_CanChangeTool = false;
	    }

	    if (Input.GetAxis("Horizontal") < -.5f && b_CanChangeTool)
	    {
            m_ActiveTools[m_CurrentToolIndex].SetActive(false);
            m_CurrentToolIndex = (m_CurrentToolIndex - 1) % m_ActiveTools.Length;
            m_ActiveTools[m_CurrentToolIndex].SetActive(true);

            m_ToolChangeTimer = 0;
            b_CanChangeTool = false;
        }

	    if (m_ToolChangeTimer > m_ChangeTimeLimit && b_CanChangeTool == false)
	        b_CanChangeTool = true;
	}
}
