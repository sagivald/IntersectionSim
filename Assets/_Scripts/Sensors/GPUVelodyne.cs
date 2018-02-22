using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GPUVelodyne : MonoBehaviour
{
    [Header("Configuration")]
    
    public float RotateFrequency = 10;
    public float AngularResolution = 0.2f, LowerVertAngle = -15, HigherVertAngle = 15; //Angular data
    public int Channels = 16; //Number of lasers
    public int SuperSample = 1; //supersample makes the velodyne scan more round
    public int ColumnsPerPhysStep;
    public float MeasurementRange = 120f;
    public float MinMeasurementRange = 0.1f;
    public float MeasurementAccuracy = 0.02f; //not used currently
    public bool RotateSensor;

    [Header("Drawing")]
    public bool DrawLidar;
    public float drawSize = 0.1f, drawTime = 0.1f;
    public Color drawColor = Color.red;
    Vector3 ray;
    Vector3 pos;

    //calculated attributes
    float horizontalFOV, verticalFOV; //calculated FOV for the camera
    int ResWidth = 24, ResHeight = 16; // calculatedcamera resolution
    float verticalAngularResolution = 0.2f; //calculated vertical resolution

    float camAngleOffsetX = 0;
    //object references
    VelodyneWrapper velo;
    Camera depthCam;
    Transform depthCamTrans, myTrans;
    GPULidar Sensor;
    Texture2D RangesSamples;
    RenderTexture rt;
    // Use this for initialization
    void Awake()
    {
        //get object references
        myTrans = transform;
        Sensor = GetComponentInChildren<GPULidar>();
        depthCam = Sensor.GetComponent<Camera>();
        depthCamTrans = Sensor.transform;
        
        //angles calculation
        
        verticalFOV = HigherVertAngle - LowerVertAngle;
        verticalAngularResolution = verticalFOV / (Channels - 1f);
        horizontalFOV = Time.fixedDeltaTime * 360 * RotateFrequency / SuperSample;
        Sensor.verticalFOV = verticalFOV;
        Sensor.horizontalFOV = horizontalFOV;
        Sensor.autoFOV();
        ColumnsPerPhysStep = Mathf.RoundToInt(Time.fixedDeltaTime * 360 * RotateFrequency / AngularResolution) / SuperSample;

        //setting up the camera
        ResWidth = ColumnsPerPhysStep;
        ResHeight = Channels;
        rt = new RenderTexture(ResWidth, ResHeight, 1, RenderTextureFormat.RFloat, RenderTextureReadWrite.Default);
        depthCam.targetTexture = rt;
        depthCam.nearClipPlane = MinMeasurementRange;
        depthCam.farClipPlane = MeasurementRange;
        depthCam.layerCullSpherical = true;

        cameraRotationAngle = horizontalFOV / 2;
        camAngleOffsetX = -(HigherVertAngle + LowerVertAngle) / 2;
        depthCamTrans.localEulerAngles = new Vector3(camAngleOffsetX, cameraRotationAngle, 0);
        cameraRotationAngle = depthCamTrans.localEulerAngles.y;
        RangesSamples = new Texture2D(ResWidth, ResHeight, TextureFormat.RGBAFloat, false);
        tempRanges = new Color[ResHeight * ResWidth];


        //ICD initialization
        if (BenjaminICD)
        {
            velo = new VelodyneWrapper(127, 0, 0, 1, Port, (int)(AngularResolution * 1000), 37, 22, Mathf.RoundToInt(RotateFrequency), 16);
            velo.Run();
        }
        sendObj = new UDPSend();
        sendObj.IP = IP;
        sendObj.port = Port;
        sendObj.init();
        ranges = new byte[1206];
        tempTime = new byte[4];
        tempAngle = new byte[2];
        tempDist = new byte[2];
    }

    Color[] tempRanges; //the object read for ranges
    float SensorAng = 0; //the angle reported to the ICD
    float cameraRotationAngle = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int s = 0; s < SuperSample; s++)
        {
            depthCam.Render();
            RenderTexture.active = rt;
            RangesSamples.ReadPixels(new Rect(0, 0, ResWidth, ResHeight), 0, 0);
            RangesSamples.Apply();
            tempRanges = RangesSamples.GetPixels();
            fireTime = Mathf.RoundToInt(Time.fixedTime * 1000000);
            
            float hAng = -horizontalFOV / 2;
            for (int i = 0; i < ResWidth; i++) //columns
            {
                float vAng = HigherVertAngle;
                for (int j = 0; j < Channels; j++) //rows
                {
                    //Get range
                    float range = (tempRanges[j * ResWidth + i].r * MeasurementRange) / (Mathf.Cos(hAng * Mathf.Deg2Rad) * Mathf.Cos(vAng * Mathf.Deg2Rad));
                    if (range < MeasurementRange) // sensor is seeing something
                    {
                        if (DrawLidar)//Visualiazation
                        {
                            ray = Quaternion.AngleAxis(hAng, depthCamTrans.up) * Quaternion.AngleAxis(vAng, depthCamTrans.right) * (depthCamTrans.forward) * range;
                            pos = ray + myTrans.position;
                            Debug.DrawLine(pos, pos + Vector3.up * drawSize, drawColor, drawTime);
                        }
                        if (BenjaminICD) velo.SetChannel(range, 0); //Sending
                        insert(range, j, SensorAng);
                    }
                    else //seeing infinity
                    {
                        if (BenjaminICD) velo.SetChannel(0, 0);
                        insert(0, j, SensorAng);
                        range = 0;
                    }
                    SensorAng = hAng + cameraRotationAngle + horizontalFOV;
                    if (SensorAng >= 360) SensorAng -= 360;
                    if (SensorAng < 0) SensorAng += 360;
                    vAng -= verticalAngularResolution;
                }
                hAng += AngularResolution;
                if (BenjaminICD)
                {
                    velo.SetAzimuth(SensorAng);
                    double timeStamp = Time.fixedTime * 1000000.0 + fireTime++;
                    velo.SetTimeStamp((int)timeStamp);
                    velo.SendData();
                }
            }
            if (RotateSensor)
            {
                cameraRotationAngle = depthCamTrans.localEulerAngles.y;
                depthCamTrans.Rotate(0, horizontalFOV, 0);
            }
        }
    }

    //-----------------------------------------------------------------//
    //Sending Data in ICD
    int blockLoc = 0, block = 0;
    bool encstamped = false;
    int dataIndex = 0;
    int fireTime = 0;
    byte[] ranges;

    public UDPSend sendObj = new UDPSend();
    public string IP = "192.168.1.201";
    public int Port = 2368;

    public bool BenjaminICD = false;
    byte[] tempTime, tempAngle, tempDist;
    ushort angle;
    private void insert(float distance, int j, float currentangle)
    {
        blockLoc = 0 + block * (16 * 6 + 4);

        if (blockLoc >= 1200)
        {
            ranges[1205] = 34;
            ranges[1204] = 54;
            tempTime = BitConverter.GetBytes(fireTime);
            System.Buffer.BlockCopy(tempTime, 0, ranges, 1200, 4);
            fireTime += 1333;
            sendObj.sendData(ranges);
            // Debug.Log("sent");
            // Debug.Log(BitConverter.IsLittleEndian);
            blockLoc = 0;
            block = 0;
            // donescan = true;
            encstamped = false;
            // Debug.Log("sent");
        }

        // ushort dist = 22921;
        ushort dist = (ushort)Mathf.Round(distance / 0.002f);
        // ushort dist = (ushort)2000;
        if (j == 0 && !encstamped) //block stamp
        {
            ranges[blockLoc] = 255;
            ranges[blockLoc + 1] = 238;
            angle = (ushort)Mathf.Round(currentangle * 100);
            tempAngle = BitConverter.GetBytes(angle);
            System.Buffer.BlockCopy(tempAngle, 0, ranges, blockLoc + 2, 2);
            // Debug.Log((ushort)Mathf.RoundToInt(currentangle * 100));
            encstamped = true;
        }

        int putInArray = j <= 7 ? j * 2 : (j - 8) * 2 + 1;
        if (block < 12)
        {
            tempDist = BitConverter.GetBytes(dist);
            System.Buffer.BlockCopy(tempDist, 0, ranges, blockLoc + 4 + putInArray * 3 + dataIndex * 16 * 3, 2);
            ranges[blockLoc + 6 + putInArray * 3 + dataIndex * 16 * 3] = 0;
        }//intensity

        if (j == 15 && dataIndex == 0) dataIndex = 1;
        else if (j == 15 && dataIndex == 1)
        {
            encstamped = false;
            dataIndex = 0;
            block++;
        }
    }
}
