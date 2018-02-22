using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelCollidersSync : MonoBehaviour {
 
  public WheelCollider wheelC;
 
  private Vector3 wheelCCenter;
  private RaycastHit hit;
  public Vector3 Offset=Vector3.zero;
  Transform myref,wheelCTransform;
  void Start () {
    myref=transform;
    wheelCTransform=wheelC.transform;
    if(!wheelC) wheelC=GetComponent<WheelCollider>();
  }
 
  void Update () {
    wheelCCenter = wheelCTransform.TransformPoint(wheelC.center+Offset);
 
    if ( Physics.Raycast(wheelCCenter, -wheelCTransform.up, out hit, wheelC.suspensionDistance + wheelC.radius) ) {
      myref.position = hit.point + (wheelCTransform.up * wheelC.radius);
    } else {
      myref.position = wheelCCenter - (wheelCTransform.up * wheelC.suspensionDistance);
    }
          myref.localEulerAngles = new Vector3(myref.localEulerAngles.x, wheelC.steerAngle - myref.localEulerAngles.z, myref.localEulerAngles.z);
 
         myref.Rotate(wheelC.rpm / 60 * 360 * Time.deltaTime, 0, 0);
  }
}
 