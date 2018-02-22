using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobcatArm : MonoBehaviour
{
    public ConfigurableJoint Arm;
    public HingeJoint loader, brackets;
    public float armpos = 0.05f, loaderpos = -15;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

            armpos=Mathf.Clamp(armpos,-0.10f,0.52f);
            armpos+=0.1f*Time.fixedDeltaTime*Input.GetAxis("Arm");
        if (Arm)
        {
            Arm.targetPosition = new Vector3(armpos, armpos, armpos);
        }
        loaderpos+=15f*Time.fixedDeltaTime*Input.GetAxis("Loader");
        loaderpos=Mathf.Clamp(loaderpos,-110,10);
        var temp = loader.spring;
        temp.targetPosition = loaderpos;
        loader.spring = temp;
    }
}
