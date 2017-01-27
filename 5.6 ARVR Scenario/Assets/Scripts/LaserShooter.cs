using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
	private LineRenderer m_Beam;
	private ShootLaser m_BeamScript;
	private float m_BeamLength;
	private float m_BeamSpeed;
	
	private bool b_CanChangeColor;
	private int m_CurrentColorIndex;
	private float m_ColorChangeTimer;
	private Light m_ColorIndicator;


	[SerializeField] private GameObject m_LaserBeamPrefab;
	[SerializeField] private Transform m_BarrelTipPosition;

	[SerializeField] private float m_MaxBeamLength;
	[SerializeField] private float m_MaxChargeTime;
	[SerializeField] private float m_MaxBeamSpeed;
	[SerializeField] private float m_MinBeamSpeed;

	[SerializeField] private ParticleSystem m_ChargingEffect;
	[SerializeField] private Color[] m_AvailableColors;
	[SerializeField] private float m_ColorChangeTimeLimit;
	[SerializeField] private LayerMask[] m_AvailableLaserLayers;

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

		switch (m_FireState)
		{
			case FireState.None:
				if (Input.GetButtonDown("Fire1"))
				{
					CmdFireLaser();
				}
				break;

			case FireState.Charging:
				if (Input.GetButtonDown("Fire1") || m_BeamLength >= m_MaxBeamLength)
				{
					m_BeamScript.length = m_BeamLength;
					m_FireState = FireState.Firing;

					// set beam length back to zero for next beam creation.
					m_BeamLength = 0;
					break;
				}

				m_BeamLength += m_MaxBeamLength * Time.deltaTime / m_MaxChargeTime;
				break;

			case FireState.Firing:
				if (Input.GetButtonDown("Fire1") || m_BeamSpeed <= m_MinBeamSpeed)
				{
					if (m_ChargingEffect != null)
						m_ChargingEffect.gameObject.SetActive(false);

					if (m_Laserbeam != null)
					{
						m_Laserbeam.transform.position = m_BarrelTipPosition.position;
						m_Laserbeam.transform.rotation = m_BarrelTipPosition.rotation;

						m_BeamScript.FireLaser(
							this.GetComponentInParent<ScoreKeeper>().gameObject,
							this.m_BeamSpeed,
							this.m_BeamLength,
							this.m_AvailableLaserLayers[this.m_CurrentColorIndex],
							this.m_AvailableColors[this.m_CurrentColorIndex]);
					}
					m_FireState = FireState.None;
					break;
				}

				// increase or decrease beam speed?
				m_BeamSpeed -= .1f;
				break;
		}

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
		m_CurrentColorIndex = (m_CurrentColorIndex + indexAmount) % m_AvailableColors.Length;

		if (m_ColorIndicator != null)
		{
			m_ColorIndicator.color = m_AvailableColors[m_CurrentColorIndex];
		}

		b_CanChangeColor = false;
		m_ColorChangeTimer = 0;
	}

	[Command]
	public void CmdFireLaser()
	{
		m_Laserbeam = Instantiate(m_LaserBeamPrefab);
		m_BeamScript = m_Laserbeam.GetComponent<ShootLaser>();

		m_Laserbeam.transform.position = m_BarrelTipPosition.position;
		m_Laserbeam.transform.rotation = m_BarrelTipPosition.rotation;

		if (m_ChargingEffect != null)
		{
			m_ChargingEffect.gameObject.SetActive(true);
		}

		m_FireState = FireState.Charging;

		m_BeamScript.FireLaser(
			this.GetComponentInParent<ScoreKeeper>().gameObject,
			this.m_MinBeamSpeed,
			this.m_MaxBeamLength,
			this.m_AvailableLaserLayers[m_CurrentColorIndex],
			this.m_AvailableColors[m_CurrentColorIndex]
			);

		//Spawn on Server
		NetworkServer.Spawn(m_Laserbeam);
	}
}
