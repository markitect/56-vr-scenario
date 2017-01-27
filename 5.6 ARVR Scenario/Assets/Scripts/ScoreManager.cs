using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreManager : NetworkBehaviour
{

	[SyncVar]
	private float score;

	public const float rate = 10;

	private List<ScoreKeeper> m_ScoreKeepers = new List<ScoreKeeper>(); 

	// Use this for initialization
	void Start () {
	}

	public void RegisterScoreKeeper(ScoreKeeper keeper)
	{
		m_ScoreKeepers.Add(keeper);
	}

	// Update is called once per frame
	void Update () {

		if(!isServer)
			return;

		float currentScore = 0;
		for (int x = 0; x < m_ScoreKeepers.Count; x++)
		{
			currentScore += m_ScoreKeepers[x].GetScore();
		}

		score = currentScore;
	}
}
