using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class WaypointDriver : MonoBehaviour
{
    public DriverScript driver;

    List<Transform> TransformList = new List<Transform>();
    public Transform Target;
    public List<Vector3> WaypointsOnTerrain=new List<Vector3>();
    Transform myref;
    [Range(-1, 2)]
    float maxAng, GPSupdateTime = 0, Velocity, PathAddTime = 0;
    Vector3 GPSposition, RealPos;
    public int WaypointID = 0;
    public float Steer = 0, Throttle = 0,
     SteerMult = 0.5f, ThrottleMult = 0.1f, TargetDistThrottleMultipler = 0.5f,
     PositionUpdateTime = 0.1f;
    public float reqVel = 5;
	bool drive=false;
    public bool AutoStart=false;

    // Use this for initialization
    private UnityAction someListener;

    void Awake ()
    {
        someListener = new UnityAction (missionStart);
    }

    void OnEnable ()
    {
        EventManager.StartListening ("Start Mission", someListener);
    }

    void OnDisable ()
    {
        EventManager.StopListening ("Start Mission", someListener);
    }

    void Start()
    {
        myref = transform;
        GPSposition = myref.position + new Vector3(Random.value, Random.value, 0) * PositionError;
        driver = GetComponent<DriverScript>();
        maxAng = driver.MaxSteering;
		Target=transform;
        if(AutoStart)init();
        
    }
    public float PositionError = 0.5f, TargetMinDistance = 4;
    Vector3 targetLocLocal = Vector3.zero;
    void missionStart(){Invoke("init", 1);}
    void init()
    {
        var gos = GameObject.FindGameObjectsWithTag(StringToFind).OrderBy(go => go.name).ToArray(); ;
        foreach (var item in gos)
        {
            RaycastHit hit;
            Physics.Raycast(item.transform.position+1000*Vector3.up,-Vector3.up, out hit);
            WaypointsOnTerrain.Add(hit.point);
            TransformList.Add(item.transform);
            Debug.Log(item.name);
        }
        Target = TransformList[WaypointID];
		drive=true;
		driver.ManualInput=false;
		
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(drive)targetLocLocal = myref.InverseTransformPoint(WaypointsOnTerrain[WaypointID]);
        GPS();
        ThrottleManagment();
        TargetMagnet();
       if(drive&&reqVel>0) driver.Drive(Throttle, Mathf.Clamp(Steer, -maxAng, maxAng));
		else driver.Drive(-1, 0);
    }
    private void GPS()
    {
        if (Time.time > GPSupdateTime + PositionUpdateTime)
        {
            GPSposition = myref.position + new Vector3(Random.value, 0, Random.value) * PositionError;
            GPSupdateTime = Time.time;
        }
        RealPos = myref.position;
        if(drive)Target = TransformList[WaypointID];
        Velocity=driver.ForwardVel;

    }
    private void ThrottleManagment()
    {
        float velDiff = ThrottleMult * (reqVel - Velocity);
        float targetDiff = Mathf.Clamp((targetLocLocal.magnitude - TargetMinDistance)* TargetDistThrottleMultipler, -2, 0.1f) ;
        Throttle = Mathf.Clamp(velDiff, -1, 0.5f) + targetDiff;
        if (targetLocLocal.magnitude < TargetMinDistance) Throttle = -1;

    }
Vector3 dist ;

    public string StringToFind = "Waypoint";

    private void TargetMagnet()
    {
        if(drive)dist = WaypointsOnTerrain[WaypointID] - GPSposition;
        if (drive&&dist.magnitude < TargetMinDistance && WaypointID == TransformList.Count- 1) drive=false;
        if (drive&&Vector3.ProjectOnPlane(dist, Vector3.up).magnitude < TargetMinDistance && WaypointID < TransformList.Count - 1) WaypointID++;
        float ang = Mathf.Deg2Rad * Vector3.Angle(Vector3.ProjectOnPlane(myref.forward, Vector3.up), Vector3.ProjectOnPlane(dist, Vector3.up));
        ang = Vector3.Cross(Vector3.ProjectOnPlane(myref.forward, Vector3.up), Vector3.ProjectOnPlane(dist, Vector3.up)).y <= 0 ? ang : -ang;
        Steer = ang * SteerMult;
        // Debug.Log(ang);
    }
    public void ChangeVelocity(float vel){
        reqVel=vel;
    }
}
