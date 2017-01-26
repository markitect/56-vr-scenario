using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreKeeper : NetworkBehaviour
{
    [SerializeField] private float rate;

    [SyncVar]
	public int score;

    public float Rate
    {
        get { return rate; }
    }

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
        
        Debug.Log("You scored!  Score is " + score);
    }

	public float GetScore()
	{
		return score;
	}
}
