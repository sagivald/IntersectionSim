using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEmitter : MonoBehaviour {
public GameObject ObjectToSpawn;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Insert)) Emit();
	}
	public void Emit(){
		Instantiate(ObjectToSpawn,transform.position,Quaternion.identity);
	}
}
