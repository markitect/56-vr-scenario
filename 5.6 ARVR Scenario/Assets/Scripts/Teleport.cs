using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	public Transform playerTransform; 

	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
		Vector3 start = this.gameObject.transform.position;
		Vector3 end = (this.transform.forward * 20) + start;

		RaycastHit hit;
		if (Physics.Raycast(start, end, out hit))
		{
			if(hit.collider.gameObject.name == "TeleportPlatform")
			{
				playerTransform.position = hit.collider.gameObject.transform.position;
				playerTransform.Translate(Vector3.up * 0.5f);
			}
		}
	}
}
