using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobcatAnimation : MonoBehaviour {
public Animator rightAnim,leftAnim;
public float Multiplier=0.001f;
TankDriver driver;
	// Use this for initialization
	void Start () {
		driver=GetComponent<TankDriver>();
	}
	
	// Update is called once per frame
	void Update () {
		rightAnim.SetFloat("Speed",driver.rightSpeed*Multiplier);
		leftAnim.SetFloat("Speed",driver.leftSpeed*Multiplier);
		rightAnim.speed=Mathf.Abs(driver.rightSpeed*Multiplier);
		leftAnim.speed=Mathf.Abs(driver.leftSpeed*Multiplier);
		
	}
}
