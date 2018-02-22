using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTestObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SnapToTerrain();	
	}

    private void SnapToTerrain()
    {
		var loc=transform.position;
		RaycastHit hit;
		Physics.Raycast(loc+Vector3.up*1000,-Vector3.up,out hit);
		transform.position=new Vector3(transform.position.x,hit.point.y,transform.position.z);
	}

    // Update is called once per frame
    void Update () {
		
	}
}
