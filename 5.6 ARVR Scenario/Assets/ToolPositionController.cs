using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class ToolPositionController : MonoBehaviour
{

	public Vector3 relativeLocalPositionForMobileDevices;
	// Use this for initialization
	void Start () {
			transform.localPosition = relativeLocalPositionForMobileDevices;
	}

	// Update is called once per frame
	void Update () {
		if (VRSettings.loadedDeviceName == "Oculus" || VRSettings.loadedDeviceName == "Vive")
		{
			transform.localPosition = InputTracking.GetLocalPosition(VRNode.RightHand);
			transform.localRotation = InputTracking.GetLocalRotation(VRNode.RightHand);
		}
	}
}
