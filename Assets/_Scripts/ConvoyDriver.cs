using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ConvoyDriver : MonoBehaviour
{
    // Use this for initialization
    public AckermannDriver driver;
    // public VELODYNE VLP16;
    Transform myref;
    public Transform Target;
    public int PointToFollowIndex = 10;
    Vector3 GPSposition, RealPos;
    public float MaxVelocity = 10, TimeToStop = 51;
    [Range(0, 2)]
    public float Steer = 0, Throttle = 0,
     SteerMult = 0.5f,ThrottleMult=0.1f, TargetDistThrottleMultipler = 0.5f,
     PositionUpdateTime = 0.1f, PathUpdateTime = 0.1f, PathResolution = 0.1f;
    public float PositionError = 0.5f, TargetMinDistance = 8;
    // List<Vector3> points, potentialObstaclePoints;
    float maxAng, GPSupdateTime = 0, Velocity, PathAddTime = 0;
    Vector3 targetLocLocal = Vector3.zero,FollowPoint;
    List<Vector3> PathPoints = new List<Vector3>();
    public bool drawPath=false;
    int reqIndex;
    private float reqVel;

    // Use this for initialization
    void Start()
    {
        myref = transform;
        GPSposition = myref.position + new Vector3(Random.value, Random.value, 0) * PositionError;
        driver = GetComponent<AckermannDriver>();
        maxAng = driver.MaxSteering;
        reqIndex = PointToFollowIndex;
        for (int i = 0; i < 2; i++)
        {
            PathPoints.Add(Target.position);
        }
        if (PathPoints.Count < reqIndex) PointToFollowIndex = PathPoints.Count - 1;
        else PointToFollowIndex = reqIndex;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Velocity = driver.ForwardVel;
        //update GPS
        GPS();
        PathManagment();
        ThrottleManagment();
        TargetMagnet(); //get initial angle from target
        // pass the input to the car!
        Steer = Mathf.Clamp(Steer, -maxAng, maxAng);
        if (targetLocLocal.magnitude < 5) Steer = 0;
        driver.Drive(Throttle, Mathf.Clamp(Steer, -maxAng, maxAng));
    }

    private void GPS()
    {
        if (Time.time > GPSupdateTime + PositionUpdateTime)
        {
            GPSposition = myref.position + new Vector3(Random.value, 0, Random.value) * PositionError;
            GPSupdateTime = Time.time;
        }
        RealPos = myref.position;
    }

    private void PathManagment()
    {
        targetLocLocal = myref.InverseTransformPoint(Target.position);
        
        if (Time.time > PathUpdateTime + PathAddTime)
        {
            if ((Target.position - PathPoints[PathPoints.Count - 1]).magnitude > PathResolution) PathPoints.Add(Target.position);
            PathAddTime = Time.time;
        }
        //points removal
        if (PathPoints.Count > PointToFollowIndex+1){
            int minIndex=0;   
            for (int i = 0; i < PathPoints.Count-PointToFollowIndex; i++)
        {
         if((PathPoints[i]-RealPos+myref.forward*driver.VehicleLength).sqrMagnitude-0.5f<=(PathPoints[minIndex]-RealPos+myref.forward*driver.VehicleLength).sqrMagnitude) minIndex=i;
         }
           PathPoints.RemoveRange(0,minIndex);
        }

        //check index overflow
        if (PathPoints.Count < reqIndex+1) PointToFollowIndex = PathPoints.Count - 1;
        else PointToFollowIndex = reqIndex;

        FollowPoint=PathPoints[PointToFollowIndex];
        if(drawPath){
            for (int i = 1; i < PathPoints.Count-1; i++)
            {
              Debug.DrawLine(PathPoints[i-1],PathPoints[i],Color.white,0.1f);
            }
            Debug.DrawLine(PathPoints[PointToFollowIndex],PathPoints[PointToFollowIndex]+Vector3.up,Color.red,0.01f);
        }
    }

    private void ThrottleManagment()
    {
        reqVel = Mathf.Clamp( (FollowPoint - PathPoints[PointToFollowIndex - 1]).magnitude / PathUpdateTime,0,MaxVelocity);
        float velDiff = ThrottleMult* (reqVel - Velocity);
        float targetDiff = Mathf.Clamp(targetLocLocal.magnitude-TargetMinDistance, -2, 0.2f)*TargetDistThrottleMultipler;
        Throttle = Mathf.Clamp(velDiff, -1, 0.5f) + targetDiff;
        // if (Time.time > TimeToStop && targetLocLocal.magnitude < TargetMinDistance) Throttle = -1;

    }

    private void TargetMagnet()
    {
        Vector3 dist = FollowPoint - GPSposition;
        float ang = Mathf.Deg2Rad * Vector3.Angle(Vector3.ProjectOnPlane(myref.forward, Vector3.up), Vector3.ProjectOnPlane(dist, Vector3.up));
        ang = Vector3.Cross(Vector3.ProjectOnPlane(myref.forward, Vector3.up), Vector3.ProjectOnPlane(dist, Vector3.up)).y <= 0 ? ang : -ang;
        Steer = ang * SteerMult;
        // Debug.Log(ang);
    }
}
