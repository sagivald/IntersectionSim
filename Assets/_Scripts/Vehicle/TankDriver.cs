//Written by Yossi Cohen <yossicohen2000@gmail.com>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDriver : MonoBehaviour
{
    public bool ManualInput = true;

    public Rigidbody[] LeftWheels, RightWheels;
    HingeJoint[] rightHinges, leftHinges;
    Transform myref;
    public float MaxTorque = 400,
            MaxSpeed = 400,
          MaxBreakingTorque = 0,
          MaxSteeringSpeed = 1,
          WheelFriction = 1,
          VelocityDamping = 0.05f,
          VehicleWidth = 2,
          VehicleLength = 3,
          Torque,
          HelperForce = 0,
          BreakPedal,
          ForwardVel = 0;
    float throttle, tempWheelFriction, angvel, appliedTrq, angularRequest;
    public float rightSpeed;
    public float leftSpeed;
    Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myref = transform;

        leftHinges = new HingeJoint[LeftWheels.Length];
        rightHinges = new HingeJoint[RightWheels.Length];
        for (int i = 0; i < LeftWheels.Length; i++)
        {
            leftHinges[i] = LeftWheels[i].GetComponent<HingeJoint>();
            rightHinges[i] = RightWheels[i].GetComponent<HingeJoint>();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ForwardVel = myref.InverseTransformDirection(rb.velocity).z;
        if (ManualInput)
        {
            angularRequest = -(Input.GetAxis("Horizontal"));
            angularRequest = Mathf.Clamp(angularRequest, -1, 1);
            throttle = Input.GetAxis("Vertical");

        }
        Apply(throttle, angularRequest);

    }
    public void Drive(float Throttle, float Steer)
    {
        angularRequest = Steer;
        throttle = Throttle;
    }
    public void Apply(float Throttle, float Steer)
    {
        Torque = MaxTorque * Throttle;
        rightSpeed = Throttle * MaxSpeed + Steer * MaxSteeringSpeed;
        leftSpeed = Throttle * MaxSpeed - Steer * MaxSteeringSpeed;
        rb.AddRelativeForce(0, 0, Throttle * HelperForce);
        for (int i = 0; i < rightHinges.Length; i++)
        {
            angvel = rightHinges[i].velocity;
            // Wheels[i].angularDrag = Break * MaxBreakingTorque;
            appliedTrq = Torque - Mathf.Clamp((VelocityDamping + tempWheelFriction) * ForwardVel, -MaxBreakingTorque, MaxTorque);

            var tempmotor = rightHinges[i].motor;
            tempmotor.targetVelocity = rightSpeed;
            tempmotor.force = MaxTorque;
            rightHinges[i].motor = tempmotor;
            tempmotor = leftHinges[i].motor;
            tempmotor.targetVelocity = leftSpeed;
            tempmotor.force = MaxTorque;
            leftHinges[i].motor = tempmotor;
            // Wheels[i].AddRelativeTorque(new Vector3(appliedTrq, 0, 0), ForceMode.Force);
        }
        //Steering

    }
}

