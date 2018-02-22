using UnityEngine;
using System.Collections;

public class bulletingthingy : MonoBehaviour {
    public Rigidbody rb;
	// Use this for initialization
	void Start () {
	rb=GetComponent<Rigidbody>();
	rb.velocity=new Vector3(0,0,20);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
