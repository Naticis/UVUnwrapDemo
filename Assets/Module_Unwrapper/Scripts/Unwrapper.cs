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
    private UnityClient client;
    private string fileName;

    [SerializeField]
    private GameObject ObjToSend;

    [SerializeField]
    private int sendingPort = 3004;

    private IPAddress ipAddress;
    private Socket clientSocket = null;
    private IPEndPoint ipEnd;

    private string fileDirectory;

    private void Awake()
    {
        client = GetComponent<UnityClient>();
        fileDirectory =  Application.persistentDataPath;
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
        fileName = fileDirectory + "\\MeshToFile.obj";
        ObjExporter.MeshToFile(ObjToSend.GetComponent<MeshFilter>(), fileName);
        SendFile(fileName);
    }

    private void SendFile(string fn)
    {
        //Clear socket - don't ask me why :| 
        clientSocket = null;
        Debug.LogError("Sending file started");

        //Connect to server
        if (clientSocket == null)
        {
            ipAddress = IPAddress.Parse(client.Host);
            ipEnd = new IPEndPoint(ipAddress, sendingPort);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            clientSocket.Connect(ipEnd);
            Debug.LogError("Client connected");
        }

        //Encode file
        string fileName = fn;
        byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
        byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
        byte[] fileData = File.ReadAllBytes(fileName);
        byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
        //[0]filenamelen[4]filenamebyte[*]filedata     

        fileNameLen.CopyTo(clientData, 0);
        fileNameByte.CopyTo(clientData, 4);
        fileData.CopyTo(clientData, 4 + fileNameByte.Length);

        //Send
        clientSocket.Send(clientData);

        GetFile(clientSocket);
    }

    public void GetFile(Socket clientSocket)
    {
        Debug.Log("Getting File...");
        byte[] clientData = new byte[1024 * 5000];
        int receivedBytesLen = clientSocket.Receive(clientData);
        int fileNameLen = BitConverter.ToInt32(clientData, 0);
        string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

        fileName = fileDirectory + "//" + Path.GetFileName(fileName);

        BinaryWriter bWrite = new BinaryWriter(File.Open(fileName, FileMode.Create));
        bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
        bWrite.Close();
        Debug.Log($"Saved File At {fileName}");

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        clientSocket = null;
        UnityMainThreadDispatcher.Instance().Enqueue(() => LoadObjFromFile(fileName));
    }

    public void LoadObjFromFile(string filePath)
    {
        GameObject loadedObject = new OBJLoader().Load(filePath);
        Debug.LogError("Loaded object");
    }
}
