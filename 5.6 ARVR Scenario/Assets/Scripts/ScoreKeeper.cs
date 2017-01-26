using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreKeeper : NetworkBehaviour
{

	[SyncVar]
	public int score;
	public const float rate = 1;

	public ScoreManager m_scoreManager;
	// Use this for initialization
	void Start ()
	{
		GameObject.Find("ScoreManager").GetComponent<ScoreManager>().RegisterScoreKeeper(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(!isServer)
			return;
	}

    public void Score()
    {
        score += 1;
    }

	public float GetScore()
	{
		return score;
	}
}
