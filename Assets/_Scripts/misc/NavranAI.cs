using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavranAI : MonoBehaviour
{
    public AckermannDriver driver;
    public VELODYNE VLP16;
    Transform myref;
    public Transform Target;
    Vector3 GPSposition;
    [Range(0, 2)]
    public float BaseThrottle = 0.2f, Steer = 0, Throttle = 0, SteerRepulsion = 0, SteerRepSpring = 10f;
    [Range(0, 10)]
    public float PointCloudFrequency = 10, PositionUpdateFrequqncy = 0.5f, PositionError = 1.5f, TargetReachedDistMultipler = 2;
    List<Vector3> points, potentialObstaclePoints;
    float maxAng, GPSupdateTime = 0, VelodyneUpdateTime = 0, middleObsCounter = 0,dt=0;

    public float SteerVelodynePointFactor = 0.1f, ThrottleVelodynePointFactor = 0.1f;

    public float ObstacleHeight = 0.3f, FrameWidth = 3;



    // Use this for initialization
    void Start()
    {
        myref = transform;
        GPSposition = myref.position + new Vector3(Random.value, Random.value, 0) * PositionError;
        points = new List<Vector3>();
        maxAng = driver.MaxSteering;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dt=Time.fixedDeltaTime;
        float Velocity = driver.ForwardVel;
        //update GPS
        if (Time.time > GPSupdateTime + PositionUpdateFrequqncy) GPSposition = myref.position + new Vector3(Random.value, 0, Random.value) * PositionError;
        // if (Time.time > PointCloudFrequency + VelodyneUpdateTime) {
        //     points.Clear();
        //     points.AddRange(VLP16.Points);
        //     VelodyneUpdateTime=Time.time; 
        //     // Debug.Log(points);
        // }//update velodyne
        TargetMagnet(); //get initial angle from target
        VelodyneRepulser(); //adjust according to sensor
        if ((new Vector3(GPSposition.x, 0, GPSposition.z) - new Vector3(Target.position.x, 0, Target.position.z)).magnitude < TargetReachedDistMultipler * PositionError)
        { Steer = 0; Throttle = -0.5f; SteerRepulsion = 0; } //Goal reached
        // pass the input to the car!
        Throttle = Mathf.Clamp(Throttle, -maxAng, maxAng);
        Steer = Mathf.Clamp(Steer, -maxAng, maxAng);
        driver.Drive(Throttle, Mathf.Clamp(Steer + SteerRepulsion, -maxAng, maxAng));
    }


    void VelodyneRepulser()
    {
        Vector3 tp;

        SteerRepulsion -= SteerRepSpring * SteerRepulsion*dt;
        foreach (Vector3 p in VLP16.Points)
        {
            tp = myref.InverseTransformPoint(p);
            if (tp.y > ObstacleHeight)
            {
                // Gizmos.DrawCube(tp,new Vector3(0.1f,0.1f,0.1f));
                if (tp.z < 5 && tp.z > 1 && Mathf.Abs(tp.x) < FrameWidth) Throttle -= ThrottleVelodynePointFactor*dt / tp.z;
                if (tp.z < 8 && tp.z > 0.5 && Mathf.Abs(tp.x) < FrameWidth)
                {
                    SteerRepulsion -= SteerVelodynePointFactor*dt / (tp.z * tp.x);
                    Debug.DrawLine(p, p + 0.05f * Vector3.up + 0.05f * Vector3.right, Color.blue, 0.3f, true);
                    if (Mathf.Abs(tp.x) < 0.02f && tp.z < 2.5 && tp.z > 0.2f) middleObsCounter++;
                }
                // potentialObstaclePoints.Add(tp);
            }
            if (Throttle < 0.09f)
            {
                SteerRepulsion = maxAng * Mathf.Sign(SteerRepulsion);
                // Throttle=0.09f;
            }
        }
        SteerRepulsion = Mathf.Clamp(SteerRepulsion, -maxAng * 2f, maxAng * 2f);
    }

    private void TargetMagnet()
    {

        Vector3 dir = Target.position - GPSposition;
        float dist = dir.magnitude;
        float ang = Mathf.Deg2Rad * Vector3.Angle(Vector3.ProjectOnPlane(myref.forward, Vector3.up), dir);
        ang = Vector3.Cross(Vector3.ProjectOnPlane(myref.forward, Vector3.up), dir).y < 0 ? ang : -ang;
        Steer = ang / 5;


        // Debug.Log(ang);
        Throttle = BaseThrottle;
    }
}
