using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client.Unity;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System;
using System.Net;
using Dummiesman;


public class Unwrapper : MonoBehaviour
{
    [SerializeField]
    private UnityClient client;

    [SerializeField]
    private GameObject ObjToSend;

    [SerializeField]
    private string fileName;

    private void Awake()
    {
        client = GetComponent<UnityClient>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendObjToServer();
        }
    }



    private void SendObjToServer()
    {
        using (DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            //  Write mesh to string, then to writer
            ObjExporter.MeshToFile(ObjToSend.GetComponent<MeshFilter>(), fileName);
            SendFile(fileName);
            //byte[] bytes = Encoding.ASCII.GetBytes(ObjExporter.MeshToString(ObjToSend.GetComponent<MeshFilter>()));
            //writer.Write(bytes);

            using (Message msg = Message.Create(Tags.SendObj, writer))
            {
                //Send over
                client.SendMessage(msg, SendMode.Reliable);
                Debug.Log("Send ObjtoServer");
            }
        }
    }

    private void SendFile(string fn)
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEnd = new IPEndPoint(ipAddress, 3004);
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        string fileName = fn;// "c:\\filetosend.txt";
        byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
        byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
        byte[] fileData = File.ReadAllBytes(fileName);
        byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];

        fileNameLen.CopyTo(clientData, 0);
        fileNameByte.CopyTo(clientData, 4);
        fileData.CopyTo(clientData, 4 + fileNameByte.Length);
        clientSocket.Connect(ipEnd);
        clientSocket.Send(clientData);
        clientSocket.Close();

        //[0]filenamelen[4]filenamebyte[*]filedata     
    }

    public static void LoadObjFromFile(string filePath)
    {
        GameObject loadedObject = new OBJLoader().Load(filePath);
        Debug.LogError("Loaded object");
    }
}
