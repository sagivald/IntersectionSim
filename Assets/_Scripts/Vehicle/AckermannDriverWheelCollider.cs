//Written by Yossi Cohen <yossicohen2000@gmail.com>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This code is an Ackermann vehicle using WHeel Colliders
public class AckermannDriverWheelCollider : DriverScript {
    // public bool ManualInput = true;
    // public bool BreakOnNullThrottle = true;

    public float  Throttle,
        steeringangle = 0,
        BreakForce = 1,
		WheelAngularFriction=1,
        // VelocityDamping = 0.05f,
        MaxSteer = 0.4f,
        MaxSteerSpeed = 50,
        // VehicleWidth = 2,
        // VehicleLength = 3,
        // Torque,
        InnerAngle,
        BreakPedal;
        // ForwardVel;
    private float ThetaAckerman, TempWheelFriction, angvel
, appliedTrq;
    [Tooltip("Assign the wheels you want motorized here.")]
    public WheelCollider[] Wheels;

    Transform myref;
    Rigidbody rb;
    float[] Theta = { 0, 0 }, breakAngle;
    bool[] lockWheel;
    [Tooltip("Assign the steering joints in a Left Wheel, Right Wheel, Left Wheel, Right wheel... manner")]
    public WheelCollider[] steering;
    float breakingIntegral = 0;

    // public int gear=1;



    // Use this for initialization
    void Start()
    {
        myref = transform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        ForwardVel = myref.InverseTransformDirection(rb.velocity).z;
        if (ManualInput)
        {
            steeringangle = MaxSteer * Input.GetAxis("Horizontal");
            Throttle = Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") : 0;
            BreakPedal = Input.GetAxis("Vertical") < 0 ? -Input.GetAxis("Vertical") : 0;
                        if(Input.GetButtonDown("GearDown")) gear--;
            if(Input.GetButtonDown("GearUp")) gear++;
            gear=Mathf.Clamp(gear,-1,1);
        }
        if (BreakOnNullThrottle && Throttle == 0) BreakPedal = 1;
        Apply(Throttle, steeringangle, BreakPedal);

    }
    public override void Drive(float T, float S)
    {
        Throttle = T;
        steeringangle = Mathf.Clamp(S, -MaxSteer, MaxSteer);
    }
    public void Apply(float Throttle, float Steer, float Break)
    {
        InnerAngle = Mathf.Clamp(Steer, -MaxSteer, MaxSteer);
        if(Throttle<0) BreakPedal=-Throttle;
        else if (Throttle>0.1f) BreakPedal=0;
        BreakPedal=Mathf.Clamp(BreakPedal,0,1);
        Throttle = Mathf.Clamp(Throttle, 0, 1);
        
        Torque = MaxTorque * Throttle*gear;

        for (int i = 0; i < Wheels.Length; i++)
        {
            // if (Break>0 && angvel < 0 && ForwardVel > 0) appliedTrq = 0;
            // if (Break>0 && angvel > 0 && ForwardVel < 0) appliedTrq = 0;
            Wheels[i].motorTorque=Torque;
			Wheels[i].brakeTorque=BreakPedal*BreakForce;
            // var tempmotor=joints[i].motor;
            // tempmotor.targetVelocity=Speed;
            // joints[i].motor=tempmotor;
        }
        if (InnerAngle >= 0) //turning right
        {
            ThetaAckerman = Mathf.Atan(1 / ((1 / (Mathf.Tan(InnerAngle)) + (VehicleWidth / VehicleLength))));
            Theta[0] = InnerAngle;
            Theta[1] = ThetaAckerman;
        }
        else if (InnerAngle < 0) //turning left
        {
            ThetaAckerman = Mathf.Atan(1 / ((1 / (Mathf.Tan(-InnerAngle)) + (VehicleWidth / VehicleLength))));
            Theta[0] = -ThetaAckerman;
            Theta[1] = InnerAngle;
        }
        for (int i = 0; i < steering.Length; i++)
        {
			if (Mathf.Rad2Deg*Theta[i%2]-steering[i].steerAngle>MaxSteerSpeed*Time.fixedDeltaTime)
            steering[i].steerAngle+=MaxSteerSpeed*Time.fixedDeltaTime;
			else if (Mathf.Rad2Deg*Theta[i%2]-steering[i].steerAngle<-MaxSteerSpeed*Time.fixedDeltaTime)
			steering[i].steerAngle-=MaxSteerSpeed*Time.fixedDeltaTime;
			else steering[i].steerAngle=Mathf.Rad2Deg*Theta[i%2];
        }

    }
}
