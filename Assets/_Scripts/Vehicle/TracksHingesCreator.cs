using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksHingesCreator : MonoBehaviour {
	public GameObject[] TracksR,TracksL;
	// Use this for initialization
	void Start () {
		HingeJoint hinge=TracksR[0].AddComponent<HingeJoint>();
			hinge.connectedBody=TracksR[TracksR.Length-1].GetComponent<Rigidbody>();
			hinge.anchor=Vector3.zero;
			hinge=TracksL[0].AddComponent<HingeJoint>();
			hinge.connectedBody=TracksL[TracksL.Length-1].GetComponent<Rigidbody>();
			hinge.anchor=Vector3.zero;
		for (int i = 1; i < TracksR.Length; i++)
		{
			hinge=TracksR[i].AddComponent<HingeJoint>();
			hinge.connectedBody=TracksR[i-1].GetComponent<Rigidbody>();
			hinge.anchor=Vector3.zero;
			// hinge.connectedAnchor=new Vector3(0,-0.1f,0);
			// hinge.autoConfigureConnectedAnchor=false;
		}
		for (int i = 1; i < TracksL.Length; i++)
		{
			hinge=TracksL[i].AddComponent<HingeJoint>();
			hinge.connectedBody=TracksL[i-1].GetComponent<Rigidbody>();
			hinge.anchor=Vector3.zero;
			// hinge.autoConfigureConnectedAnchor=false;
			// hinge.connectedAnchor=new Vector3(0,-0.1f,0);
		}
	}
	
}
