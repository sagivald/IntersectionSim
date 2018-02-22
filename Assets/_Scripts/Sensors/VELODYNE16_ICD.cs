//Written by Yossi Cohen <yossicohen2000@gmail.com>

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

public class VELODYNE16_ICD : MonoBehaviour
{

    WebsocketClient wsc;
    Transform myref;
    public float Timescale = 1;
    public Transform emitter, SensorRotator;
    Vector3 shootLaserDir = Vector3.zero;
    [Range(60f, 150)]    
    public float Range = 70;
    float  StartVerticalAngle = -15, VertScanAngRange = 30, HorScanAngRange = 360;
    [Range(0.1f, 2f)]
    public float HorRes = 1f;
    int NoOfScansPerFrame = 0;
    public int hz = 10;
    int lasercount = 16;
    public bool DebugDraw = false, DebugDrawDots = true;
    public float DrawTime = 0.1f;
    // float[] datacolumn;
    float pitchAngle = 0;
    public UDPSend sendObj = new UDPSend();
    Rigidbody attachedRB;
    Vector3 intLoc, ScannerLoc, prevScannerLoc, velocity = Vector3.zero;
    public bool InterpolateLocation = true, ROS = false, UDP = false;
    public string IP="192.168.1.201";
    public int Port=2368;
    // StringMsg rosMsg = new StringMsg("");

    // Use this for initialization
    void Start()
    {
        sendObj = new UDPSend();
        sendObj.IP=IP;
        sendObj.port = Port;
        sendObj.init();
        ranges = new byte[1206];
        for (int i = 0; i < ranges.Length; i++)
        {
            ranges[i] = 0;
        }
        myref = transform;
        ScannerLoc = myref.position;
        prevScannerLoc = ScannerLoc;
        if (!SensorRotator) SensorRotator = myref.Find("Laser Sensor").transform;
        if (!emitter) SensorRotator = myref.Find("Emitter").transform;

        attachedRB = GetComponentInParent<Rigidbody>();

        // writer = new StreamWriter(Application.dataPath+"/Data.csv");
        NoOfScansPerFrame = (int)((HorScanAngRange / HorRes) * hz * Time.fixedDeltaTime);
        currentangle = 0;

        // writer.WriteLine("This is a data file");
        // datacolumn=new float[lasercount];
    }
    float currentangle;
    byte[] ranges;
    // private StreamWriter writer;
    string rosData = "";
    short dataOffset = 42;
    // Update is called once per frame
    void FixedUpdate()
    {
        ScannerLoc = myref.position;
        velocity = (ScannerLoc - prevScannerLoc) / Time.fixedDeltaTime;
        //message floats format: [number of rays,number of columns, sensor Angle, min Vert Angle, vertRes,HorRes,x,y,z,rx,ry,rz[ranges]] in ros coordinates x forward y left

        byte[] data = new byte[ranges.Length * 4];//char[ranges.Length * 4];
        if (currentangle > HorScanAngRange)
        {
            currentangle = 0; SensorRotator.localEulerAngles = new Vector3(0, 0, 0);

        } //completed horizontal scan

        intLoc = ScannerLoc + velocity * Time.fixedDeltaTime;

        for (int i = 0; i < NoOfScansPerFrame; i++)
        { // multiple horizontal scans in 1 physics step in order to achieve the full range in the desired rate
            if (InterpolateLocation)
                ScannerLoc = Vector3.Lerp(myref.position, intLoc, (float)i / NoOfScansPerFrame);
            if (currentangle > HorScanAngRange)
            {
                currentangle = 0; SensorRotator.localEulerAngles = new Vector3(0, 0, 0);
            }
            emitter.localEulerAngles = new Vector3(-StartVerticalAngle, 0, 0);
            for (int j = 0; j < lasercount; j++) //the lazer column
            {
                pitchAngle = (StartVerticalAngle + j * VertScanAngRange / (lasercount - 1));
                emitter.localEulerAngles = new Vector3(pitchAngle, 0, 0);
                shootLaserDir = (emitter.forward);
                RaycastHit hit;
                if (Physics.Raycast(ScannerLoc, shootLaserDir, out hit, Range))
                {
                    Vector3 p = hit.point;
                    if (DebugDraw) Debug.DrawLine(p, SensorRotator.position, Color.red, DrawTime, true);
                    else if (DebugDrawDots) Debug.DrawLine(p, p + 0.1f * Vector3.up, Color.red, DrawTime, true);

                    insert(hit.distance, 15-j, currentangle);
                }
                else
                {


                    insert(0, 15-j, currentangle);

                    if (DebugDraw) Debug.DrawLine(SensorRotator.position, SensorRotator.position + shootLaserDir * Range, Color.blue, 0.3f, true);
                }

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

            // Debug.Log(data.Length);
        }
        prevScannerLoc = SensorRotator.position;
    }
    int blockLoc = 0, block = 0;
    bool encstamped = false;
    int dataIndex=0;
    int fireTime = 0;
    // bool donescan = false;
    private void insert(float distance, int j, float currentangle)
    {
        blockLoc = 0 + block * (16 * 6 + 4);

        if (blockLoc >= 1200)
        {
            ranges[1205] = 34;
            ranges[1204] = 54;
            System.Buffer.BlockCopy(BitConverter.GetBytes(fireTime), 0, ranges, 1200, 4);
            fireTime += 1333;
            // Debug.Log(BitConverter.GetBytes(fireTime)[3]);
            sendObj.sendData(ranges);
            // Debug.Log(ranges);
            // Debug.Log(BitConverter.IsLittleEndian);
            blockLoc = 0;
            block = 0;
            // donescan = true;
            encstamped = false;
            // Debug.Log("sent");
        }

        // ushort dist = 22921;
        ushort dist = (ushort)Mathf.RoundToInt(distance / 0.002f);
        // ushort dist = (ushort)2000;
        if (j == 0 && !encstamped) //block stamp
        {
            ranges[blockLoc] = 255;
            ranges[blockLoc + 1] = 238;
            System.Buffer.BlockCopy(BitConverter.GetBytes((ushort)Mathf.RoundToInt(currentangle * 100)), 0, ranges, blockLoc + 2, 2);
            // Debug.Log((ushort)Mathf.RoundToInt(currentangle * 100));
            encstamped = true;
        }

        int putInArray=j<=7?j*2:(j-8)*2+1;
        if(block<12){System.Buffer.BlockCopy(BitConverter.GetBytes(dist), 0, ranges, blockLoc + 4 + putInArray * 3 + dataIndex*16 * 3, 2);
        ranges[blockLoc + 6 + putInArray * 3 + dataIndex*16 * 3] = 0;}//intensity
        
        if (j == 15 && dataIndex==0) dataIndex=1;
        else if (j == 15 && dataIndex==1)
        {
            encstamped=false;
            dataIndex=0;
            block++;
        }
    }
} 