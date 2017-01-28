using System.Collections;
using System;
using UnityEngine;

public class LaserSplitter : MonoBehaviour
{

	[SerializeField]
	private GameObject laserPrefab;

	private GameObject[] prismFaces = new GameObject[3];

	// Use this for initialization
	void Start()
	{
		prismFaces[0] = this.transform.FindChild("Face1").gameObject;
		prismFaces[1] = this.transform.FindChild("Face2").gameObject;
		prismFaces[2] = this.transform.FindChild("Face3").gameObject;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SplitLaser(GameObject faceHit, ShootLaser laser)
	{
		var newSpeed = laser.speed / 2;
		var newLength = laser.length / 2;
		var newColorIndex = GetNextLaserColorIndex(laser.laserColor);
		var newColor = LaserColors.LaserColor[newColorIndex];

		foreach (GameObject face in prismFaces)
		{
			if (face.name == faceHit.name)
			{
				//do nothing with face that was hit
			}
			else
			{
				face.GetComponent<MeshRenderer>().material.color = LaserColors.LaserColor[newColorIndex];
				GameObject laserEmitter = face.transform.FindChild("Emitter").gameObject;
				var newLaser = Instantiate(laserPrefab, laserEmitter.transform.position, laserEmitter.transform.rotation).GetComponent<ShootLaser>();
				newLaser.FireLaser(laser.scorer.gameObject, newSpeed, newLength, LaserColors.LaserLayerMasks[newColorIndex], LaserColors.LaserColor[newColorIndex]);
				newColorIndex = GetNextLaserColorIndex(newColor);
			}
		}

		StartCoroutine(ResetFaceColors());
	}

	private int GetNextLaserColorIndex(Color currentLaserColor)
	{
		int colorSize = LaserColors.LaserColor.Length - 1;
		int laserPosition = Array.IndexOf(LaserColors.LaserColor, currentLaserColor);

		if (laserPosition < colorSize)
		{
			return laserPosition + 1;
		}
		else
		{
			return 0;
		}
	}

	public IEnumerator ResetFaceColors()
	{
		yield return new WaitForSeconds(3f);

		foreach (var prismFace in prismFaces)
		{
			prismFace.GetComponent<MeshRenderer>().material.color = Color.white;
		}
	}
}
