//Written by Yossi Cohen <yossicohen2000@gmail.com>
   
 using UnityEngine;
 using System.Collections;
 
 [AddComponentMenu("Camera-Control/Yossi's Smooth Follow Script")]
 
 public class CustomSmoothFollow: MonoBehaviour
 {
     public string TagToFind="Vehicle";
     /*
     This camera smoothes out rotation around the y-axis and height.
     Horizontal Distance to the target is always fixed.
     
     There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.
     
     For every of those smoothed values we calculate the wanted value and the current value.
     Then we smooth it using the Lerp function.
     Then we apply the smoothed values to the transform's position.
     */
 
     // The target we are following
     public Transform target;
	  Transform myref;
     // The distance in the x-z plane to the target
     public float distance = 10.0f;
     // the height we want the camera to be above the target
     public float height = 2.0f;
     // How much we 
     public float heightDamping = 0.2f;
     GameObject[] targets;
     int targetID=0;
	      public float rotationDamping = 3.0f;
  void  Start ()
     {
         targets=GameObject.FindGameObjectsWithTag("Vehicle");
		 myref=transform;
	 }
     void  LateUpdate ()
     {
         if(Input.GetKeyDown(KeyCode.C)) {targetID++;
         if (targetID>=targets.Length) targetID=0;
         target=targets[targetID].transform;
         }
         // Early out if we don't have a target
         if (!target){ GameObject go=GameObject.FindGameObjectWithTag(TagToFind);
         if(go)target=go.transform;
         else
             return;}
        if(!target.gameObject.activeSelf) {GameObject go=GameObject.FindGameObjectWithTag(TagToFind);
        if(go)target=go.transform;}
        if(!target) return;
         // Calculate the current rotation angles
         float wantedRotationAngle = target.eulerAngles.y;
         float wantedHeight = target.position.y + height;
         float currentRotationAngle = transform.eulerAngles.y;
         float currentHeight = transform.position.y;
     
         // Damp the rotation around the y-axis
         currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
 
         // Damp the height
         currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
 
         // Convert the angle into a rotation
         Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
     
         // Set the position of the camera on the x-z plane to:
         // distance meters behind the target
         transform.position = target.position;
         transform.position -= currentRotation * Vector3.forward * distance;
 
         // Set the height of the camera
         transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        if (Input.GetMouseButton(1))
        {
            myref.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X"));
            myref.RotateAround(target.position, -myref.right, Input.GetAxis("Mouse Y"));
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) distance--;
        if (Input.GetAxis("Mouse ScrollWheel") < 0) distance++;
        // Always look at the target
        transform.LookAt (target);
     }
 }
