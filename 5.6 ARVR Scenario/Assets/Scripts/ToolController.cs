using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class ToolController : NetworkBehaviour {

	[SerializeField]
	private Transform m_ToolPosition;
	[SerializeField]
	private GameObject[] m_AvailableTools;
	[SerializeField]
	private float m_ChangeTimeLimit;

	private GameObject[] m_ActiveTools;
	private int m_CurrentToolIndex;
	private float m_ToolChangeTimer;
	private bool b_CanChangeTool;

	public GameObject laserTest;
	private GameObject laserInstance;

	private float WaitTimeBeforeSpawning = 5.0f;
	private float time = 0.0f;

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
			return;

		b_CanChangeTool = true;

		//m_ActiveTools = new GameObject[m_AvailableTools.Length];
        laserTest = Resources.Load("ArResources/Prefabs/LaserGun") as GameObject;

		//for (int x = 0; x < m_AvailableTools.Length; x++)
		//{
		//	CmdSpawnObject(m_AvailableTools[x], x);
		//}

		CmdSpawnObject2();

		//CmdEnableTool(0);
	}

	void Awake()
	{
	}

	[Command]
	public void CmdEnableTool(int i)
	{
		m_ActiveTools[i].SetActive(true);
		for (int x = 0; x < m_ActiveTools.Length; x++)
		{
			if(x != i)
				m_ActiveTools[x].SetActive(false);
		}
	}

	//[Command]
	//public void CmdSpawnObject(GameObject obj, int i)
	//{
	//	m_ActiveTools[i] = Instantiate(obj);
	//	NetworkServer.Spawn(m_ActiveTools[i]);
	//	m_ActiveTools[i].SetActive(false);
	//}


	[Command]
	public void CmdSpawnObject2()
	{
		laserInstance = Instantiate(laserTest);
		NetworkServer.Spawn(laserInstance);
	}


	// Update is called once per frame
	void Update () {

		if(!isLocalPlayer)
			return;


		//laserInstance.transform.position = InputTracking.GetLocalPosition(VRNode.RightHand);
		//laserInstance.transform.rotation = InputTracking.GetLocalRotation(VRNode.RightHand);



		Debug.Log("Horizontal: " + Input.GetAxis("Horizontal"));

		m_ToolChangeTimer += Time.deltaTime;

		if (Input.GetAxis("Horizontal") > .5f && b_CanChangeTool)
		{
			m_CurrentToolIndex = (m_CurrentToolIndex + 1) % m_ActiveTools.Length;
			CmdEnableTool(m_CurrentToolIndex);

			m_ToolChangeTimer = 0;
			b_CanChangeTool = false;
		}

		if (Input.GetAxis("Horizontal") < -.5f && b_CanChangeTool)
		{
			m_CurrentToolIndex = (m_CurrentToolIndex - 1) % m_ActiveTools.Length;
			CmdEnableTool(m_CurrentToolIndex);

			m_ToolChangeTimer = 0;
			b_CanChangeTool = false;
		}

		if (m_ToolChangeTimer > m_ChangeTimeLimit && b_CanChangeTool == false)
			b_CanChangeTool = true;
	}
}
