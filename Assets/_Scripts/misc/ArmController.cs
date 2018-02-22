using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public HingeJoint[] Joints;
    public ConfigurableJoint[] CJoints;
    public Vector3[] Axis;
    public Rigidbody[] rb;
    public PID[] Controllers;
    public float[] Angles;
    public float[] Status, Tourque;
    public bool ConfJoints = true;
    float current = 0;
    int loopValue = 0;
    // Use this for initialization
    void Start()
    {
		if (ConfJoints)loopValue = CJoints.Length;
        else loopValue = Joints.Length;

        for (int i = 0; i < loopValue; i++)
        {
            // Angles[i] = 0;
            if (ConfJoints)
            {
                Axis[i] = (CJoints[i].axis);
                rb[i] = (CJoints[i].gameObject.GetComponent<Rigidbody>());
            }
            else
            {

                Axis[i] = (Joints[i].axis);
                rb[i] = (Joints[i].gameObject.GetComponent<Rigidbody>());
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < loopValue; i++)
        {
            if (ConfJoints){Debug.Log(jointRotation(CJoints[i])); current = -jointRotation(CJoints[i]).x;}

            Status[i] = current;
            // Debug.Log(current);
            float torque = Controllers[i].Update(Angles[i], current, Time.fixedDeltaTime);
            Tourque[i] = torque;
            rb[i].AddRelativeTorque(torque * Axis[i]);

        }
    }
    public float to180(float v)
    {
        if (v > 180)
        {
            v = v - 360;
        }
        return v;
    }
    Vector3 jointRotation(ConfigurableJoint joint)
    {
        Quaternion jointBasis = Quaternion.LookRotation(joint.secondaryAxis, Vector3.Cross(joint.axis, joint.secondaryAxis));
        Quaternion jointBasisInverse = Quaternion.Inverse(jointBasis);
        var rotation = (jointBasisInverse * Quaternion.Inverse(joint.connectedBody.rotation) * joint.GetComponent<Rigidbody>().transform.rotation * jointBasis).eulerAngles;
        return new Vector3(to180(rotation.x), to180(rotation.z), to180(rotation.y));
    }
}
