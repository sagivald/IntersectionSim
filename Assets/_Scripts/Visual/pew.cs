using UnityEngine;
using System.Collections;

public class pew : MonoBehaviour {

	// Use this for initialization
	void Start () {
	GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,10),ForceMode.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
