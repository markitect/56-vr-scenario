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

	public void SplitLaser(GameObject faceHit, Laser laser)
	{
		this.GetComponent<MeshRenderer>().material.color = LaserData.Lasers[laser.laserType].LaserColor;
		var newSpeed = laser.speed / 2;
		var newLength = laser.length / 2;
		var newLaser = GetNextLaser(laser.laserType);
		var newColor = LaserData.Lasers[newLaser].LaserColor;

		foreach (GameObject face in prismFaces)
		{
			if (face.name == faceHit.name)
			{
				//do nothing with face that was hit
			}
			else
			{
				//face.GetComponent<MeshRenderer>().material.color = LaserData.Lasers[newColorIndex];
				//GameObject laserEmitter = face.transform.FindChild("Emitter").gameObject;
				//var newLaser = Instantiate(laserPrefab, laserEmitter.transform.position, laserEmitter.transform.rotation).GetComponent<Laser>();
				//newLaser.FireLaser(laser.scorer.gameObject, newSpeed, newLength, LaserColors.LaserLayerMasks[newColorIndex], LaserColors.LaserColor[newColorIndex]);
				//newColorIndex = GetNextLaserColorIndex(newColor);
			}
		}

		StartCoroutine(ResetFaceColors());
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

	public IEnumerator ResetFaceColors()
	{
		yield return new WaitForSeconds(3f);

		foreach (var prismFace in prismFaces)
		{
			prismFace.GetComponent<MeshRenderer>().material.color = Color.white;
		}
	}
}
