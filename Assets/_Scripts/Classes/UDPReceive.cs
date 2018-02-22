using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
public class UDPReceive : MonoBehaviour
{
    // receiving Thread
    Thread receiveThread;
    // udpclient object
    UdpClient client;

    // public
    // public string IP = "127.0.0.1"; default local
    public int port = 5050; // define > init
    public int ByteBufferSize = 50;
    // infos
    string lastReceivedUDPPacket = "";
    string allReceivedUDPPackets = ""; // clean up this from time to time!

    // start from shell
    private static void Main()
    {
        UDPReceive receiveObj = new UDPReceive();
        receiveObj.init();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    // start from unity3d
    public void Start()
    {
        init();
    }

 

    // init
    private void init()
    {
        dataPackets=new List<byte[]>();
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");
        // define port
        // port = 5005;
        // status
        print("Sending to 127.0.0.1 : " + port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");

        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }
    bool clear = false;
    byte[] ByteData;
    int writeIndex = 0;
    byte[] lastData=new byte[0];
    bool stopped = false;
    List<byte[]> dataPackets;
    // receive thread
    private void OnApplicationQuit()
    {
        stopped = true;
        
    }
    private void ReceiveData()
    {
        ByteData = new byte[ByteBufferSize];

        client = new UdpClient(port);
        while (true)
        {
            if (stopped) return;
            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes  in  Textformat.
                // string text = Encoding.UTF8.GetString(data);
                dataPackets.Add(data);
                if (dataPackets.Count > ByteBufferSize) dataPackets.RemoveAt(0);

                // latest UDPpacket
                // lastReceivedUDPPacket = text;

                // Thread.Sleep(1);
            }
            catch (Exception)//Exception err)
            {
                throw;
                // print(err.ToString());
            }


        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
    public byte[] getUDPData()
    {
        if(dataPackets.Count>0){lastData = dataPackets[0];
        dataPackets.RemoveAt(0);}
        else lastData=null;
        return lastData;
    }
    public int GetBufferState()
    {
        return dataPackets.Count;
    }
       // OnGUI
    // void OnGUI()
    // {
    //     Rect rectObj = new Rect(40, 10, 200, 400);
    //     GUIStyle style = new GUIStyle();
    //     style.alignment = TextAnchor.UpperLeft;
    //     GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
    //                 + "shell> nc -u 127.0.0.1 : " + port + " \n"
    //                 + "\nLast Packet: \n" + lastReceivedUDPPacket
    //                 + "\n\nAll Messages: \n" + allReceivedUDPPackets
    //             , style);
    // }
}