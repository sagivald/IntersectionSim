using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeJumper : MonoBehaviour {
    public float JumpForce=1,a;
    public Rigidbody rbToJump;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetButtonDown("Jump"))
        {
            rbToJump.AddForce(new Vector3(0, JumpForce, 0));
            rbToJump.AddTorque(new Vector3(Random.value * 10, Random.value * 10, Random.value * 10));
        }
	}
}
