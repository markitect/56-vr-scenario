using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreKeeper : NetworkBehaviour
{

	[SyncVar]
	private float score;
	public const float rate = 10;

	private LaserShooter m_ShootLaser;
	public ScoreManager m_scoreManager;
	// Use this for initialization
	void Start ()
	{
		m_ShootLaser = GetComponent<LaserShooter>();
		GameObject.Find("ScoreManager").GetComponent<ScoreManager>().RegisterScoreKeeper(this);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isServer)
			return;

		if (m_ShootLaser.isScoring)
		{
			score += rate * Time.deltaTime;
		}
	}

	public float GetScore()
	{
		return score;
	}
}
