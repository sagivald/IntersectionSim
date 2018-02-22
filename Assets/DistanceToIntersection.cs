using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class DistanceToIntersection : MonoBehaviour {
    public int ID = 0;
    public LayerMask layers;
public float Distance,forwardOffset=0;
Transform MyTransform;
	// Use this for initialization
	void Start () {
		MyTransform=transform;	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        RaycastHit hit;
        if (Physics.Raycast(transform.position+forwardOffset* transform.forward, transform.forward, out hit, 200, layers, QueryTriggerInteraction.Collide))
            Distance = hit.distance;
        Debug.DrawLine(transform.position + forwardOffset * transform.forward, hit.point, Color.red);
	}
}
