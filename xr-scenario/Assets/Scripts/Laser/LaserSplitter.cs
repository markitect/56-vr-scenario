using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class LaserSplitter : MonoBehaviour
{
    [SerializeField]
    private GameObject[] faces;

    private LaserShooter laserShooter;
    
    private bool _isInUse = false;
    public bool IsInUse
    {
        get
        {
            return _isInUse;
        }

        set
        {
            if (value == false)
            {
                this.ResetFaceColors();
            }

            this._isInUse = value;
        }
    }

    // Use this for initialization
    void Start()
	{
        this.laserShooter = this.GetComponent<LaserShooter>();
	}

	// Update is called once per frame
	void Update()
	{
	}
    
	public void SplitLaser(GameObject faceHit, Laser laser)
	{
        this.IsInUse = true;
		faceHit.GetComponentInParent<MeshRenderer>().material.color = LaserData.Lasers[laser.laserType].LaserColor;
		var newSpeed = laser.speed / 2;
		var newLength = laser.length / 2;
        var newLaserType = GetNextLaser(laser.laserType);

        foreach (var face in this.faces)
        {
            if (face != faceHit)
			{
                var emitter = face.GetComponentInChildren<Transform>();
                face.GetComponent<MeshRenderer>().material.color = LaserData.Lasers[newLaserType].LaserColor;

                this.laserShooter.SetColor(newLaserType);
                if (face.transform.Find("Emitter") != null)
                {
                    this.laserShooter.m_BarrelTipPosition = face.transform.Find("Emitter");
                }

                this.GetComponent<LaserShooter>().CmdFireLaser();

                newLaserType = GetNextLaser(newLaserType);
            }
		}
	}

	private LaserType GetNextLaser(LaserType laserType)
	{
		var laserArraySize = Enum.GetValues(typeof(LaserType)).Length;
		var laserPosition = (int)laserType;

		if (laserPosition < laserArraySize)
		{
			return (LaserType)(laserPosition + 1);
		}
		else
		{
			return (LaserType)0;
		}
	}
    
	public void ResetFaceColors()
	{
		foreach (var face in faces)
		{
            face.GetComponentInParent<MeshRenderer>().material.color = Color.white;
		}
	}
}
