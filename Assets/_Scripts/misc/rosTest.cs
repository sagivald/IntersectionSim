using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.sensor_msgs;


public class rosTest : MonoBehaviour {
	private ROSBridgeWebSocketConnection ros = null;
	public GameObject Vehicle;	
	public VELODYNE VelodyneScanner;
	public string topic = "/ros_unity";
	public float scale = 1f;
	public float VelodyneRate=10;
	float timeToPublishVelodyne=1;
	// Use this for initialization
	void Start ()
	{
	ros = new ROSBridgeWebSocketConnection ("ws://127.0.0.1", 9090);
	ros.AddPublisher(typeof(PubTest));
	ros.AddPublisher(typeof(VelodyneStringPublisher));
	ros.Connect ();
	
	// Use this for initialization
		
	}
	
	// Update is called once per frame
	void Update () {
		// wsc.Publish(topic,"hello");
		Float32Msg msg= new Float32Msg(Vehicle.GetComponent<AckermannDriver>().ForwardVel);
		ros.Publish(PubTest.GetMessageTopic() ,msg);

	}
	void FixedUpdate() {
		if(Time.time > timeToPublishVelodyne){
		string points="";
		int i=0;
		foreach(Vector3 p in VelodyneScanner.Points)
		{
			i++;
			points+=p.x.ToString()+","+p.y.ToString("F3")+","+p.z.ToString()+";";
		}
		Debug.Log("messages sent: "+i);
		ros.Publish(VelodyneStringPublisher.GetMessageTopic(),new StringMsg(points));
		// 	PointFieldMsg PointsField=new PointFieldMsg("name",0,7,(uint)VelodyneScanner.Points.Count);
		// 	byte[] array=new byte[VelodyneScanner.Points.Count];
			// PointCloud2Msg VeloPoints=new PointCloud2Msg(new HeaderMsg(0,new TimeMsg(1,1),"velodyne"),120,120, PointsField,true,1,1,array,true);
		// 	// PointCloudMsg VeloPoints=new PointCloudMsg(new HeaderMsg(0,new TimeMsg(1,1),"velodyne"),1,1, PointsField,false,false,);
			
		// 	ros.Publish(VelodynePublisher.GetMessageTopic(),VeloPoints);
		}
	}
		void OnApplicationQuit() {
		if(ros!=null)
			ros.Disconnect ();
	}
}
