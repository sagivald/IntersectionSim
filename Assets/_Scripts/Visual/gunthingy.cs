using UnityEngine;
using System.Collections;

public class gunthingy : MonoBehaviour {
	public GameObject boomboom;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	if(Input.GetButton("Fire1"))Instantiate(boomboom,transform.position,Quaternion.identity);
	}
}
