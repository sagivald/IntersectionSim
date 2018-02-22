using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PointCloudUDPReceiver : MonoBehaviour
{
    public int Port = 5050;
    public string IP = "127.0.0.1";
    UDPReceive udpR;
    // Use this for initialization
    void Start()
    {

        udpR = GetComponent<UDPReceive>();
        udpR.port = Port;

    }
    Vector3[] points;
    Color[] colors;
    float[] floats;
    // Update is called once per frame
    public float TimeToWait = 0.1f;
    float timestamp = 0;
    int pclIndex = 0;
    public GameObject Sprite;
    byte[] colorBytes = new byte[4];
    Vector3 size = new Vector3(0.02f, 0.02f, 0.02f);
    int drawIndex = 0;
    void Update()
    {
        // Debug.Log(udpR.GetBufferState());
        for (int r = 0; r < udpR.GetBufferState(); r++)
        {
            try
            {
                // if(Time.time>timestamp+TimeToWait)
                // {
                byte[] data = udpR.getUDPData();
                if (data == null) break;
                floats = new float[data.Length / 4 + 1];
                points = new Vector3[floats.Length / (8)];
                colors = new Color[points.Length];
                System.Buffer.BlockCopy(data, 0, floats, 0, data.Length);
                // Debug.Log(floats[0]);
                for (int i = 0; i < points.Length; i++)
                {
                    System.Buffer.BlockCopy(data,i*4*8+16,colorBytes,0,4);
                    //  System.BitConverter.GetBytes(floats[i * 8 + 4]);
                    //  var r = (bigint >> 16) & 255;
                    //  var g = (bigint >> 8) & 255;
                    //  var b = bigint & 255;
                    //  Debug.Log(floats[i * 8+4]);
                    points[i] = new Vector3(floats[i * 8], floats[i * 8 + 1], floats[i * 8 + 2]);
                    colors[i] = new Color32(colorBytes[2], colorBytes[1], colorBytes[0], colorBytes[3]);
                    // if(drawIndex%20==0)if(!float.IsNaN(points[i].x)){GameObject cube = Instantiate(Sprite,points[i],Quaternion.identity);
                    // cube.GetComponent<SpriteRenderer>().color=colors[i];
                    // Destroy(cube,0.5f);}
                    // drawIndex++;
                    // if(verbose)Debug.DrawLine(points[i],points[i]+0.05f*Vector3.up,new Color(floats[i*6+3],floats[i*6+4],floats[i*6+5]),0.1f);
                }
                // pclIndex = 1;//Mathf.RoundToInt(floats[floats.Length-1]);
                // Debug.Log(pclIndex);
                CallBack();
                // timestamp=Time.time;
                // }

            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
    Vector3[] pts = new Vector3[meshSize];
    int[] indecies = new int[meshSize];
    Color[] vertColors = new Color[meshSize];
    public void UpdateMesh(ref Mesh m, int firstPointIndex, int meshSize = 65000)
    {
        /* 
            Recieves a point cloud msg and makes a mesh out of it
            Makes a single mesh the size of meshSize with pass marking which part of the cloud we are making
            Dilutes the point cloud index-wise if the point cloud size is higher than expected mesh size

            inputs: 
                pointCloud - as the name suggests
                pass - iteration pass / part of the cloud we are making
                meshSize - unity allows max of 65,534 vertecies per mesh, do not set higher than this

            TODO optimisations:
                [pcl.size] instead of [pcl.Points.Count], not sure if they both return the same, minor optimisation either way
         */

        // Mesh mesh = new Mesh();
        int pointCount = points.Length;

        // mesh components
        firstPointIndex -= meshSize * (firstPointIndex / meshSize);
        // build the mesh with colors
        for (int idx = 0, inidx = firstPointIndex; inidx < meshSize && idx < pointCount; idx++, inidx++)
        {
            pts[inidx] = points[idx];
            indecies[inidx] = inidx;
            vertColors[inidx] = colors[idx];
        }

        // apply the mesh
        m.vertices = pts;
        m.colors = vertColors;
        m.SetIndices(indecies, MeshTopology.Points, 0);

        // return mesh;
    }
    public void MakeMesh(ref Mesh m, int meshSize = 65000)
    {
        int pointCount = points.Length;

        // mesh components
        Vector3[] pts = new Vector3[meshSize];
        int[] indecies = new int[meshSize];
        Color[] colors = new Color[meshSize];
        // apply the mesh
        m.vertices = pts;
        m.colors = colors;
        m.SetIndices(indecies, MeshTopology.Points, 0);

        // return mesh;
    }

    public bool verbose = false; // show logging
    int gameObjectsToMake = 0;
    int gameObjectsToDelete = 0;
    int objectCount = 0;
    List<GameObject> Containers = new List<GameObject>();
    const int meshSize = 65000; // unity allows max of 65,534
    const string shaderName = "Custom/VertexColor";
    public int neededObjectCount = 10;
    public void CallBack()
    {
        int pointCount = points.Length;

        // check if there are existing game objects and how many to make if needed
        objectCount = Containers.Count;

        gameObjectsToMake = neededObjectCount - objectCount;

        gameObjectsToDelete = objectCount - neededObjectCount;


        // logging
        if (verbose) Debug.Log("PCLSUB: recieved points " + pointCount + " ;objects existing " + objectCount +
        " ;making objects " + gameObjectsToMake +
        " ;needed objects " + neededObjectCount + " ;deleting " + gameObjectsToDelete);

        // delete uneeded game objects
        // for (int idx = gameObjectsToDelete; idx > 0; idx--)
        // {
        //     Destroy(Containers[Containers.Count - 1]);
        //     Containers.RemoveAt(Containers.Count - 1);
        // }

        // create game objects
        for (int idx = 0; idx < gameObjectsToMake; idx++)
        {
            GameObject currentObject = new GameObject("PointCloud");
            currentObject.AddComponent<MeshFilter>();
            currentObject.GetComponent<MeshFilter>().mesh = new Mesh();
            Mesh me = currentObject.GetComponent<MeshFilter>().mesh;
            MakeMesh(ref me, meshSize);
            currentObject.AddComponent<MeshRenderer>();
            currentObject.GetComponent<MeshRenderer>().allowOcclusionWhenDynamic = false;
            currentObject.GetComponent<Renderer>().material.shader = Shader.Find(shaderName); // set our vertex shader
            // currentObject.tag = "PointCloud"; // important to find these objects later on(not used anymore)

            currentObject.transform.SetParent(transform, false);
            currentObject.transform.position = Vector3.zero; // only needed if you intend to change pointcloud transform via parent
            Containers.Add(currentObject);
        }

        // --- Applying The Meshes ---

        if (pclIndex - meshSize * (pclIndex / meshSize) + pointCount > meshSize) pclIndex += meshSize - (pclIndex % meshSize);
        if (pclIndex / meshSize > neededObjectCount - 1)
        {
            pclIndex = 0;
        }
        Mesh m = Containers[pclIndex / meshSize].GetComponent<MeshFilter>().mesh;
        UpdateMesh(ref m, pclIndex);
        pclIndex += pointCount;

    }
}

