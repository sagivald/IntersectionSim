using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverScript : MonoBehaviour {
    public bool ManualInput = true;
    public bool BreakOnNullThrottle = true;
    public int gear=1;
    public float MaxTorque = 400,
        MaxBreakingTorque = 800,
		MaxSteering=0.4f,
        VelocityDamping = 0.05f,
        VehicleWidth = 2,
        VehicleLength = 3,
        Torque=0,

        ForwardVel = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    public virtual void Drive(float T, float S)
    {

    }
    public virtual void Drive(float T, float S, float breaks, float steerSpeed)
    {

    }
}
