using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TriggerEvent : MonoBehaviour {
public bool TriggerEnter=false;
public string EventString;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame

public void Trigger(string E){
	EventManager.TriggerEvent(E);
}
private void OnTriggerEnter(Collider other) {
	if (TriggerEnter)
	{
		EventManager.TriggerEvent(EventString);
	}
}
}
