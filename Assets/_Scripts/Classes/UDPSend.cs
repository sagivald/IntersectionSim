using System.Collections;
using System.Collections.Generic;
using System;
// using UnityEngine;Z
using System.Net;
using System.Net.Sockets;
using System.Text;
public class UDPSend  {
	// Use this for initialization
  private static int localPort;

    // prefs
    public string IP="127.0.0.1";  // define in init
    public int port=5050;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    string strMessage="";       

    // call it from shell (as program)
    // private static void Main()
    // {
        // UDPSend sendObj=new UDPSend();
        // sendObj.init();

        // testing via console
        // sendObj.inputFromConsole();

        // as server sending endless
        // sendObj.sendEndless(" endless infos \n");           
    // }
    // start from unity3d
    public void Start()
    {
        init();
    }

    // OnGUI 
   

    // init
    public void init()
    {
        // Define end point , from which the messages are sent.
        // print("UDPSend.init()");

        // define
        // IP="127.0.0.1";
        // port=5050;

        // Send
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        // print("Sending to "+IP+" : "+port);
        // print("Testing: nc -lu "+IP+" : "+port);
        // sendString("jt");
    }


    // sendData
    public void sendString(string message)
    {
        try
        {
            //if (message != "")
            //{
                
                byte[] data = Encoding.UTF8.GetBytes(message);

            // Send the message to the remote client.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            // print(err.ToString());
        }
    }       
    public void sendData(byte[] data)
    {
        try
        {
            // Send the message to the remote client.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            // print(err.ToString());
        }
    }     
    // endless test
    private void sendEndless(string testStr)
    {
        do 
        {
            sendString(testStr);    
        }
        while(true);            
    }       
}

