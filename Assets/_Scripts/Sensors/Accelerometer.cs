using UnityEngine;
using System.Collections;

public class Accelerometer : MonoBehaviour {
    public Vector3 RealAccel,Accel=Vector3.zero,lastvel;
	public Rigidbody RbToMonitor;
	Transform myref;
	// Use this for initialization
	void Start () {
	myref=transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 locvel=myref.InverseTransformVector(RbToMonitor.velocity);
			if((RbToMonitor.velocity-lastvel)!=Vector3.zero) RealAccel=(locvel-lastvel)/Time.fixedDeltaTime;
		Accel=RealAccel+myref.InverseTransformVector(Physics.gravity);
	lastvel=locvel;
	}
}
