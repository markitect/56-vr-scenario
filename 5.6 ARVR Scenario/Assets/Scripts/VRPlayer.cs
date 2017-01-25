using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class VRPlayer : NetworkBehaviour
{
    [SerializeField] private Transform m_ToolPosition;
    [SerializeField] private GameObject[] m_AvailableTools;
    [SerializeField] private float m_ChangeTimeLimit;

    private GameObject[] m_ActiveTools;
    private int m_CurrentToolIndex;
    private float m_ToolChangeTimer;
    private bool b_CanChangeTool;

	public Camera childCamera;
    public PlayerRoles role { get; private set; }

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

        b_CanChangeTool = true;

        if (VRSettings.loadedDeviceName == "HoloLens")
        {
            gameObject.AddComponent<ARPlayer>();
            role = PlayerRoles.Blocker;
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
