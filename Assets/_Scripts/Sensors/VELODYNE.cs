//Written by Yossi Cohen <yossicohen2000@gmail.com>

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;

// using ROSBridgeLib;
// using ROSBridgeLib.std_msgs;
public class VELODYNE : MonoBehaviour
{

    WebsocketClient wsc;
    Transform myref;
    public float Timescale = 1;
    public Transform emitter, SensorRotator;
    Vector3 shootLaserDir = Vector3.zero;
    public float Range = 10, StartVerticalAngle = 2, VertScanAngRange = 26.8f, HorScanAngRange = 360, hz = 10, HorRes = 0.08f;
    int NoOfScansPerFrame = 0;
    public int lasercount = 64;
    public bool DebugDraw = true, DebugDrawDots = false;
    public float DrawTime=0.1f;
    // float[] datacolumn;
    public List<Vector3> Points;
    List<Vector3> PointsTemp;
    float pitchAngle = 0;
    public UDPSend sendObj = new UDPSend();
    Rigidbody attachedRB;
    Vector3 intLoc, ScannerLoc, prevScannerLoc, velocity = Vector3.zero;
    public bool InterpolateLocation = true, ROS = false, UDP = false;
    // StringMsg rosMsg = new StringMsg("");

    // Use this for initialization
    void Start()
    {
        myref = transform;
        ScannerLoc = myref.position;
        prevScannerLoc = ScannerLoc;
        if (!SensorRotator) SensorRotator = myref.Find("Laser Sensor").transform;
        if (!emitter) SensorRotator = myref.Find("Emitter").transform;

        attachedRB = GetComponentInParent<Rigidbody>();
        if (UDP && !ROS)
        {
            // sendObj =gameObject.AddComponent< UDPSend>();
            sendObj=new UDPSend();
            sendObj.init();
        }
        PointsTemp = new List<Vector3>();
        Points = new List<Vector3>();
        // writer = new StreamWriter(Application.dataPath+"/Data.csv");
        NoOfScansPerFrame = (int)((HorScanAngRange / HorRes) * hz * Time.fixedDeltaTime);
        currentangle = -HorScanAngRange / 2;
        if (ROS)
        {
            wsc = FindObjectOfType<WebsocketClient>();
            wsc.Advertise("velodyne", "std_msgs/String");
            // SimulationManager.instance.ros.AddPublisher(typeof(VelodyneStringPublisher));

        }
        // writer.WriteLine("This is a data file");
        // datacolumn=new float[lasercount];
    }
    float currentangle;
    float[] ranges;
    // private StreamWriter writer;
    string rosData = "";
    short dataOffset = 14;
    // Update is called once per frame
    void FixedUpdate()
    {
        ScannerLoc = myref.position;
        velocity = (ScannerLoc - prevScannerLoc) / Time.fixedDeltaTime;
        //message floats format: [number of rays,number of columns, sensor Angle, min Vert Angle, vertRes,HorRes,x,y,z,rx,ry,rz[ranges]] in ros coordinates x forward y left
        ranges = new float[NoOfScansPerFrame * lasercount + dataOffset];
        byte[] data = new byte[ranges.Length * 4];//char[ranges.Length * 4];
        if (currentangle > HorScanAngRange / 2)
        {
            currentangle = -HorScanAngRange / 2; SensorRotator.localEulerAngles = new Vector3(0, -HorScanAngRange / 2, 0);
            // if (!ROS && !UDP) Points = PointsTemp;
            // if (!ROS && !UDP) PointsTemp.Clear();
            // if (ROS){ 
            // SimulationManager.instance.ros.Publish("velodyne/string", rosMsg);
            // }
            // rosData = "";

        } //completed horizontal scan
        ranges[0] = lasercount;
        ranges[1] = NoOfScansPerFrame;
        ranges[2] = currentangle * Mathf.Deg2Rad;
        ranges[3] = StartVerticalAngle * Mathf.Deg2Rad;
        ranges[4] = ((VertScanAngRange) / (lasercount-1)) * Mathf.Deg2Rad;
        ranges[5] = HorRes * Mathf.Deg2Rad;
        Vector3 rosVector= RosUnityCoordinates.UnityToRosPositionAxisConversion(myref.position);
        Quaternion rosRot=RosUnityCoordinates.UnityToRosRotationAxisConversion(myref.rotation);
        ranges[6] = rosVector.x;
        ranges[7] = rosVector.y;
        ranges[8] = rosVector.z;
        ranges[9] = rosRot.x;
        ranges[10] = rosRot.y;
        ranges[11] = rosRot.z;
        ranges[12] = rosRot.w;
        ranges[13] = HorScanAngRange* Mathf.Deg2Rad;
        intLoc = ScannerLoc + velocity*Time.fixedDeltaTime;

        for (int i = 0; i < NoOfScansPerFrame; i++)
        { // multiple horizontal scans in 1 physics step in order to achieve the full range in the desired rate
            if (InterpolateLocation)
                ScannerLoc = Vector3.Lerp(myref.position, intLoc, (float)i / NoOfScansPerFrame);
            if (currentangle > HorScanAngRange / 2) { currentangle = -HorScanAngRange / 2; SensorRotator.localEulerAngles = new Vector3(0, -HorScanAngRange / 2, 0); }//rotate horizontally
            emitter.localEulerAngles = new Vector3(-StartVerticalAngle, 0, 0);
            for (int j = 0; j < lasercount; j++) //the lazer column
            {
                pitchAngle = (StartVerticalAngle + j * VertScanAngRange / (lasercount-1));
                emitter.localEulerAngles = new Vector3(pitchAngle, 0, 0);
                shootLaserDir = (emitter.forward);
                RaycastHit hit;
                if (Physics.Raycast(ScannerLoc, shootLaserDir, out hit, Range))
                {
                    Vector3 p = hit.point;
                    if (!ROS) PointsTemp.Add(p);
                    if (DebugDraw) Debug.DrawLine(p, SensorRotator.position, Color.red, DrawTime, true);
                    else if (DebugDrawDots) Debug.DrawLine(p, p + 0.1f * Vector3.up, Color.red, DrawTime, true);

                    if (ROS || UDP)
                    {
                        ranges[j + i * lasercount + dataOffset] = hit.distance;
                    }
                }
                else
                {
                    if (ROS || UDP)
                    {
                        ranges[j + i * lasercount + dataOffset] = 0;
                    }
                    if (DebugDraw) Debug.DrawLine(SensorRotator.position, SensorRotator.position + shootLaserDir * Range, Color.blue, 0.3f, true);
                }
                // datacolumn[j]=hit.distance;
            }

            // WriteData(datacolumn);
            currentangle += HorRes;
            SensorRotator.localEulerAngles = new Vector3(0, currentangle, 0);

        }

        if (ROS)
        {

            System.Buffer.BlockCopy(ranges, 0, data, 0, data.Length);

            string d = (data.ToString());
            Debug.Log(d);
            wsc.Publish("velodyne", d);
        }
        if (UDP && !ROS)
        {
            System.Buffer.BlockCopy(ranges, 0, data, 0, data.Length);

            sendObj.sendData(data);
            // Debug.Log(data.Length);
        }
        prevScannerLoc = SensorRotator.position;
    }

}














    //     void WriteData(float[] toWrite){
    //         for (int n = 0; n < toWrite.Length; n++)
    //         {
    //             writer.Write(string.Format("{0}, ",
    //                          toWrite[n])); 
    //         }

    //     writer.WriteLine();

    // }


