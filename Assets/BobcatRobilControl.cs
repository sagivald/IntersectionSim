using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobcatRobilControl : MonoBehaviour
{
    public float Steering = 0;
    TankDriver driver;
    public float Throttle;
    float TimeToStop = 0;
    WebsocketClient wsc;
    // Use this for initialization
    void Start()
    {
        driver = GetComponent<TankDriver>();
        driver.ManualInput = false;
        wsc = FindObjectOfType<WebsocketClient>();
        wsc.Subscribe("LLC/EFFORTS/Steering", "std_msgs/Float64", 0);
        wsc.Subscribe("LLC/EFFORTS/Throttle", "std_msgs/Float64", 0);
    }
    private void OnApplicationQuit()
    {
        // wsc.Unsubscribe("a");
        wsc.Unsubscribe("LLC/EFFORTS/Steering");
        wsc.Unsubscribe("LLC/EFFORTS/Throttle");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Debug.Log(wsc.messages["a"]);
        try
        {
            Steering = float.Parse(wsc.messages["LLC/EFFORTS/Steering"]);
            TimeToStop = Time.time + 1;

        }
        catch (System.Exception)
        {

        }
        try
        {
            Throttle = float.Parse(wsc.messages["LLC/EFFORTS/Throttle"]);
            TimeToStop = Time.time + 1;
        }
        catch (System.Exception)
        {

        }
        driver.Drive(Throttle, Steering);
        if (Time.time > TimeToStop)
        {
            Throttle = 0;
            Steering = 0;
        }
    }
}

