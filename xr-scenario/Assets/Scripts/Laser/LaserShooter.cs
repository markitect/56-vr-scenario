using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class LaserShooter : NetworkBehaviour
{
	private enum FireState
	{
		None,
		Charging,
		Firing
	}
	
	private FireState m_FireState;
	private GameObject m_Laserbeam;
	private float m_BeamLength;
	private float m_BeamSpeed;
	
	private bool b_CanChangeColor;
	private float m_ColorChangeTimer;
	private Light m_ColorIndicator;

	private LaserType currentLaserType;
	public LaserSettings laserSettings;

	public LaserColorInfo CurrentLaserData
	{
		get
		{
			return LaserData.Lasers[this.currentLaserType];
		}
	}

    public Transform m_BarrelTipPosition;

    [SerializeField] private GameObject m_LaserBeamPrefab;

	[SerializeField] private ParticleSystem m_ChargingEffect;
	[SerializeField] private float m_ColorChangeTimeLimit;

	void Start()
	{
		b_CanChangeColor = true;
		m_ColorIndicator = gameObject.GetComponentInChildren<Light>();
	}

	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		m_ColorChangeTimer += Time.deltaTime;

		if (Input.GetButtonDown("Fire1"))
		{
			CmdFireLaser();
		}

        //switch (m_FireState)
        //{
        //    case FireState.None:
        //        if (Input.GetButtonDown("Fire1"))
        //        {
        //            CmdFireLaser();
        //        }
        //        break;

        //        //case FireState.Charging:
        //        //	if (Input.GetButtonDown("Fire1") || m_BeamLength >= m_MaxBeamLength)
        //        //	{
        //        //		m_BeamScript.length = m_BeamLength;
        //        //		m_FireState = FireState.Firing;

        //        //		// set beam length back to zero for next beam creation.
        //        //		m_BeamLength = 0;
        //        //		break;
        //        //	}

        //        //	m_BeamLength += m_MaxBeamLength * Time.deltaTime / m_MaxChargeTime;
        //        //	break;

        //        //case FireState.Firing:
        //        //	if (Input.GetButtonDown("Fire1") || m_BeamSpeed <= m_MinBeamSpeed)
        //        //	{
        //        //		if (m_ChargingEffect != null)
        //        //			m_ChargingEffect.gameObject.SetActive(false);

        //        //		if (m_Laserbeam != null)
        //        //		{
        //        //			m_Laserbeam.transform.position = m_BarrelTipPosition.position;
        //        //			m_Laserbeam.transform.rotation = m_BarrelTipPosition.rotation;

        //        //			m_BeamScript.FireLaser(
        //        //				this.GetComponentInParent<ScoreKeeper>().gameObject,
        //        //				this.m_BeamSpeed,
        //        //				this.m_BeamLength,
        //        //				this.m_AvailableLaserLayers[this.m_CurrentColorIndex],
        //        //				this.m_AvailableColors[this.m_CurrentColorIndex]);
        //        //		}
        //        //		m_FireState = FireState.None;
        //        //		break;
        //        //	}

        //        //	// increase or decrease beam speed?
        //        //	m_BeamSpeed -= .1f;
        //        //	break;
        //}

        if (Input.GetAxis("Vertical") > .5 && b_CanChangeColor)
		{
			ChangeColor(1);
		}

		if (Input.GetAxis("Vertical") < -.5 && b_CanChangeColor)
		{
			ChangeColor(-1);
		}

		if (m_ColorChangeTimer > m_ColorChangeTimeLimit)
		{
			b_CanChangeColor = true;
		}
	}

	public void ChangeColor(int indexAmount)
	{
		this.currentLaserType = (LaserType)(((int)this.currentLaserType + indexAmount) % LaserData.Lasers.Count);

		if (m_ColorIndicator != null)
		{
			m_ColorIndicator.color = LaserData.Lasers[this.currentLaserType].LaserColor;
		}

		b_CanChangeColor = false;
		m_ColorChangeTimer = 0;
	}

    public void SetColor(LaserType laserType)
    {
        this.currentLaserType = laserType;

        if (m_ColorIndicator != null)
        {
            m_ColorIndicator.color = LaserData.Lasers[this.currentLaserType].LaserColor;
        }

        b_CanChangeColor = false;
        m_ColorChangeTimer = 0;
    }

	[Command]
	public void CmdFireLaser()
	{
		m_Laserbeam = Instantiate(m_LaserBeamPrefab, m_BarrelTipPosition.position, m_BarrelTipPosition.rotation);

		//Spawn on Server
		NetworkServer.Spawn(m_Laserbeam);

		m_Laserbeam.GetComponent<Laser>().FireLaser(
			this.GetComponent<ScoreKeeper>(),
			this.laserSettings.MinLaserSpeed,
			this.laserSettings.MaxLaserDistance,
			this.currentLaserType
			);
	}
}
