//Written by Yossi Cohen <yossicohen2000@gmail.com>


using UnityEngine;
using System.Collections;
//This code is an Ackermann vehicle using Configurable joints for physcs and steering and rigidbody torque.
public class AckermannDriver : DriverScript
{
         WebsocketClient wsc;

    public float 
        MaxSteeringSpeed = 1,SteeringAngleCommand = 0,
        InnerWheelSteeringAngle,
        BreakPedal=0;
    public float throttle=0;
    private float thetaAckerman, steeringSpeed = 10,AutoBreak=0;
    [Tooltip("Assign the wheels you want motorized here with a rotation axis set as X.")]
    // public Rigidbody[] Wheels; //Assign here the wheels you want motorized
    // HingeJoint[] hinges;
    Transform myref;
    Rigidbody rb;
    float[] Theta = { 0, 0 };
    // bool[] lockWheel;
    [Tooltip("Assign the steering configurable joints in a Left Wheel, Right Wheel, Left Wheel, Right wheel... manner With the main axis set as X")]
    public ConfigurableJoint[] steering; //Set steering joints as Left Wheel, Right Wheel, Left Wheel, Right wheel via the inspector
                                         // float breakingIntegral = 0;
    string rosTopic;
    bool rosConnetion;
    DriveTrain driveTrain;
    // Use this for initialization
    void Start()
    {
        driveTrain=GetComponent<DriveTrain>();
        wsc=FindObjectOfType<WebsocketClient>();
        rosConnetion = wsc!=null && wsc.IsConnected();
        if(rosConnetion){
            rosTopic=gameObject.name+"/Speed";
        wsc.Advertise(rosTopic,"std_msgs/Float32");}
        if (MaxBreakingTorque == 0) MaxBreakingTorque = MaxTorque;
        // hinges = new HingeJoint[Wheels.Length];
        // // lockWheel = new bool[Wheels.Length];
        // for (int i = 0; i < Wheels.Length; i++) hinges[i] = Wheels[i].GetComponent<HingeJoint>();
        myref = transform;
        rb = GetComponent<Rigidbody>();
        steeringSpeed = MaxSteeringSpeed;
    }
    private void Update()
    {
        if (ManualInput)
        {
            if(Input.GetButtonDown("GearDown")) driveTrain.ShiftUp();
            if(Input.GetButtonDown("GearUp")) driveTrain.ShiftDown();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        ForwardVel = myref.InverseTransformDirection(rb.velocity).z;
        if(rosConnetion)wsc.PublishData(rosTopic,ForwardVel.ToString("F4"));
        // Debug.Log(ForwardVel.ToString("F4"));
        if (ManualInput)
        {
            float SteeringManualFactor=ForwardVel>0?Mathf.Clamp(5/ForwardVel,0,1):1;

            SteeringAngleCommand = -MaxSteering * SteeringManualFactor * Input.GetAxis("Horizontal");
            throttle = Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") : 0;
            BreakPedal = Input.GetAxis("Vertical") < 0 ? -Input.GetAxis("Vertical") : 0;
            steeringSpeed=MaxSteeringSpeed;
        }
        if (BreakOnNullThrottle && throttle == 0) {AutoBreak += Time.fixedDeltaTime*(1-AutoBreak);
        BreakPedal=AutoBreak;
        }
        else AutoBreak=0;
        Apply(throttle, SteeringAngleCommand, BreakPedal);
    }
    public override void Drive(float T, float S)
    {
        throttle = T;
        SteeringAngleCommand = Mathf.Clamp(S, -MaxSteering, MaxSteering);
        BreakPedal = T > 0 ? 0 : Mathf.Clamp(-T, 0, 1);
        steeringSpeed = MaxSteeringSpeed;
    }
    public override void Drive(float T, float S, float breaks, float steerSpeed)
    {
        throttle = T;
        SteeringAngleCommand = Mathf.Clamp(S, -MaxSteering, MaxSteering);
        BreakPedal = breaks;
        steeringSpeed =Mathf.Clamp(steerSpeed,0,MaxSteeringSpeed);
    }
    public void Apply(float Throttle, float Steer, float Break)
    {
       
        // Throttle = Mathf.Clamp(Throttle, 0, 1);
        // Torque = MaxTorque * Throttle*gear;
        // for (int i = 0; i < Wheels.Length; i++)
        // {
        //     angvel = hinges[i].velocity;
       
        //     appliedTrq =Mathf.Clamp( Torque - (tempWheelFriction) * Mathf.Clamp(angvel,-1,1), -MaxBreakingTorque, MaxTorque); //Torque calculation

        //     Wheels[i].AddRelativeTorque(new Vector3(appliedTrq, 0, 0), ForceMode.Force);
        // }

        //Steering
        driveTrain.throttle=Throttle;
        driveTrain.throttleInput=Throttle;
        driveTrain.BreakPedal=Break;
        Steer = Mathf.Clamp(Steer, -MaxSteering, MaxSteering);
        if (Mathf.Abs(InnerWheelSteeringAngle - Steer) < steeringSpeed * 1.5f*Time.deltaTime) InnerWheelSteeringAngle = Steer;
        else InnerWheelSteeringAngle = InnerWheelSteeringAngle > Steer ? InnerWheelSteeringAngle - steeringSpeed*Time.fixedDeltaTime : InnerWheelSteeringAngle + steeringSpeed*Time.fixedDeltaTime;
        ApplySteering();
    }
    void ApplySteering()
    {
        InnerWheelSteeringAngle=Mathf.Clamp(InnerWheelSteeringAngle, -MaxSteering, MaxSteering);
        if (InnerWheelSteeringAngle >= 0) //turning right
        {
            thetaAckerman = Mathf.Atan(1 / ((1 / (Mathf.Tan(InnerWheelSteeringAngle)) + (VehicleWidth / VehicleLength))));
            Theta[0] = InnerWheelSteeringAngle;
            Theta[1] = thetaAckerman;
        }
        else if (InnerWheelSteeringAngle < 0) //turning left
        {
            thetaAckerman = Mathf.Atan(1 / ((1 / (Mathf.Tan(-InnerWheelSteeringAngle)) + (VehicleWidth / VehicleLength))));
            Theta[0] = -thetaAckerman;
            Theta[1] = InnerWheelSteeringAngle;
        }
        for (int i = 0; i < steering.Length; i++)
        {
            var temp = steering[i].targetRotation;
            if (i % 2 == 0) temp = Quaternion.Euler(Mathf.Rad2Deg * Theta[0], 0, 0);
            else temp = Quaternion.Euler(Mathf.Rad2Deg * Theta[1], 0, 0);
            steering[i].targetRotation = temp;
        }
    }
}
