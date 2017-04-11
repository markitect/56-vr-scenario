using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHead : MonoBehaviour {
	private Transform cameraTransform;

	// Use this for initialization
	void Start () {
		cameraTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow)) 
			cameraTransform.Rotate(Vector3.right, -2.5f);
		else if (Input.GetKeyDown(KeyCode.DownArrow)) 
			cameraTransform.Rotate(Vector3.right, 2.5f); 
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) 
			cameraTransform.Rotate(cameraTransform.InverseTransformVector(Vector3.up), -2.5f); 
		else if (Input.GetKeyDown(KeyCode.RightArrow)) 
			cameraTransform.Rotate(cameraTransform.InverseTransformVector(Vector3.up), 2.5f);
	}
}
