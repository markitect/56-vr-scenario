using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LaserShooter : NetworkBehaviour
{
    public Color LaserColor;

    [SerializeField] private GameObject m_LaserBeamPrefab;
    [SerializeField] private Transform m_BarrelTipPosition;
    [SerializeField] private float m_MaxBeamLength;
    [SerializeField] private float m_MaxChargeTime;
    [SerializeField] private ParticleSystem m_ChargingEffect;

    private enum FireState
    {
        None,
        Charging,
        Firing
    }
    
    private FireState m_FireState;
    private GameObject m_Laserbeam;
    private float m_BeamLength;
    private LineRenderer m_Beam;
    private float m_BeamSpeed;

	public bool isScoring;

    void Update()
    {
        switch (m_FireState)
        {
            case FireState.None:
                if (Input.GetButtonDown("Fire1"))
                {
                    m_Laserbeam = Instantiate(m_LaserBeamPrefab);
                    m_Beam = m_Laserbeam.GetComponent<LineRenderer>();
                    m_Beam.numPositions = 2;
                    m_Beam.GetComponent<Transform>().position = m_BarrelTipPosition.position;
                    m_Beam.SetPosition(0, Vector3.zero);
                    m_Beam.startColor = Color.blue;
                    m_Beam.endColor = m_Beam.startColor;

                    if (m_ChargingEffect != null)
                        m_ChargingEffect.gameObject.SetActive(true);

                    m_FireState = FireState.Charging;
                }
                break;

            case FireState.Charging:
                if (Input.GetButtonDown("Fire1") || m_BeamLength >= m_MaxBeamLength)
                {
                    m_Beam.SetPosition(1, new Vector3(0, 0, m_BeamLength));
                    m_FireState = FireState.Firing;

                    // set beam length back to zero for next beam creation.
                    m_BeamLength = 0;
                    break;
                }

                // this may have to change to better conform to the beam control system.
                // but basically the beam should get longer the longer a player holds the button here.
                m_BeamLength += m_MaxBeamLength * Time.deltaTime / m_MaxChargeTime;
                break;

            case FireState.Firing:
                if (Input.GetButtonDown("Fire1"))
                {
                    if (m_ChargingEffect != null)
                        m_ChargingEffect.gameObject.SetActive(false);

                    // TODO: fire event Mark is working which should be attached to the beam prefab.
                    // should set the speed of the beam here too.
                    Debug.Log("Fired laser!");
                    m_FireState = FireState.None;
                    break;
                }

                // increase or decrease beam speed?
                m_BeamSpeed -= 5;
                break;
        }
    }
}
