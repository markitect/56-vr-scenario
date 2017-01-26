using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LaserSplitter : MonoBehaviour {

    [SerializeField]
    private GameObject laserPrefab;

    private GameObject[] prismFaces = new GameObject[3];

	// Use this for initialization
	void Start () {
        prismFaces[0] = this.transform.FindChild("Face1").gameObject;
        prismFaces[1] = this.transform.FindChild("Face2").gameObject;
        prismFaces[2] = this.transform.FindChild("Face3").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SplitLaser(GameObject faceHit, Color laserColor, float laserSpeed)
    {
        float newSpeed = laserSpeed / 2;
        Color currentColor = laserColor;

        foreach (GameObject face in prismFaces)
        {
            if (face.name == faceHit.name)
            {
                //do nothing with face that was hit
            }
            else
            {
                GameObject laserEmitter = face.transform.FindChild("Emitter").gameObject;
                var laser = Instantiate(laserPrefab, laserEmitter.transform.position, laserEmitter.transform.rotation).GetComponent<ShootLaser>();
                laser.speed = newSpeed;
                currentColor = GetNextLaserColor(currentColor);
                laser.laserColor = currentColor;
                laser.FireLaser();
            }
        }
    }

    private Color GetNextLaserColor(Color currentLaserColor)
    {
        int colorSize = LaserColors.LaserColor.Length - 1;
        int laserPosition = Array.IndexOf(LaserColors.LaserColor, currentLaserColor);
        
        if(laserPosition < colorSize)
        {
            return LaserColors.LaserColor[laserPosition + 1];
        }
        else
        {
            return LaserColors.LaserColor[0];
        }
    }
}
