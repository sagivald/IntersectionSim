using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDistanceView : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Terrain.activeTerrain.detailObjectDistance = 500;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
