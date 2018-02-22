using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.Threading;
public class MinDistanceBetweenMeshes : MonoBehaviour
{
    public MeshFilter Mesh1, Mesh2;
    Mesh mesh1, mesh2;
    Matrix4x4 mat1, mat2;
    Transform Trans1, Trans2;
    float mindist = 100;
    Vector3 mindistV = Vector3.zero, dist = Vector3.zero;
    Vector3[] vertices1, vertices2;
    Vector3 pos1 = Vector3.zero, pos2 = Vector3.zero;
	float t1=0,t2=0;

    // WebsocketClient wsc;
    // Use this for initialization
    void Start()
    {
        // wsc=FindObjectOfType<WebsocketClient>();
        // wsc.Advertise("topic","geometry_msgs/Quaternion");
        mesh1 = Mesh1.mesh;
        mesh2 = Mesh2.mesh;
        vertices1 = mesh1.vertices;
        vertices2 = mesh2.vertices;
        Trans1 = Mesh1.transform;
        Trans2 = Mesh2.transform;
        InvokeRepeating("DrawDist", 1, 1);
    }
    private static readonly Object obj = new Object();
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            // Quaternion q=new Quaternion(1.342f,2.3f,2,1);
            // Debug.Log(UnityEngine.JsonUtility.ToJson(q));
            // wsc.Publish("topic",UnityEngine.JsonUtility.ToJson(q));
            // StartCoroutine(GetDist());

            // lock (obj)
            // {
            //     // thread unsafe code
            //     mat1.SetTRS(Trans1.position, Trans1.rotation, Trans1.localScale);
            //     mat2.SetTRS(Trans2.position, Trans2.rotation, Trans2.localScale);
            // }
            var thread = new Thread(GetDist);
            thread.Start();
        }

    }
    // Update is called once per frame
    void GetDist()
    {
		t1=System.DateTime.Now.Second*1000+System.DateTime.Now.Millisecond;
        Matrix4x4 m1 = mat1;
        Matrix4x4 m2 = mat2;
		mindist=100;
        while (true)
        {
            var a=1;
            Thread.Sleep(1);
        }
        foreach (var item1 in vertices1)
        {
            foreach (var item2 in vertices2)
            {
                dist = m1.MultiplyPoint(item1) - m2.MultiplyPoint(item2);
                float distance = dist.magnitude;
                if (distance < mindist)
                {
                    mindist = distance;
                    pos1 = m1.MultiplyPoint(item1);
                    pos2 = m2.MultiplyPoint(item2);
                    mindistV = Vector3.Scale(dist, new Vector3(1, 0, 1));

                }
            }
            // yield return new WaitForSeconds(0.01f);
        }
		t2=System.DateTime.Now.Second*1000+System.DateTime.Now.Millisecond;

    }
    void DrawDist()
    {
        Debug.Log(vertices1.Length+vertices2.Length);
        // Debug.Log(t2-t1);
        // Debug.DrawLine(pos1, pos2, Color.black, 1);
        // Debug.DrawRay(pos2, mindistV, Color.red, 1);
    }
}
