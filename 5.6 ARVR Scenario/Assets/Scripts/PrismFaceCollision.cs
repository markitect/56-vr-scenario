using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PrismFaceCollision : MonoBehaviour {

    public GameObject laserPrefab;

    [SerializeField]
    private GameObject Emitter;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SplitLaser()
    {
        var laser = Instantiate(laserPrefab, Emitter.transform.position, Emitter.transform.rotation).GetComponent<ShootLaser>();
        laser.speed = 1;
        laser.FireLaser(); 
    }
}
