using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 
public class TrafficLightManager : MonoBehaviour {
    TrafficLight[] Lights,OrderedLights;

    public int[] States;
    public float[] Timers= { 1, 1, 1, 1 };
	// Use this for initialization
	void Start () {
        Lights = FindObjectsOfType<TrafficLight>();
        States = new int[Lights.Length];
        OrderedLights = new TrafficLight[Lights.Length];
        for (int i = 0; i < Lights.Length; i++)
        {
            States[Lights[i].ID] = Lights[i].state;
            OrderedLights[Lights[i].ID] = Lights[i];
        }

	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < Lights.Length; i++)
        {
            OrderedLights[i].state = States[i];
        }
    }
    public void SetStates(int a, int b, int c, int d)
    {
        States[0] = a;
        States[1] = b;
        States[2] = c;
        States[3] = d;
    }
}
