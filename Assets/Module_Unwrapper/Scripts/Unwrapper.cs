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
            RealTimeUnwrap(ObjToSend);
        }
    }

    /// <summary>
    /// Takes the current target, sends it to server, server open blender with python script, unwraps, and send the item back.
    /// Then spawns it into the scene.
    /// </summary>
    public void RealTimeUnwrap(GameObject target)
    {
        fileName = fileDirectory + "\\MeshToFile.obj";
        ObjExporter.MeshToFile(target.GetComponent<MeshFilter>(), fileName);
        SendFile(fileName);
    }

    /// <summary>
    /// Sends file to server
    /// DarkRift must connect first.
    /// </summary>
    /// <param name="fn">File to be sent, with path, name and extension.</param>
    private void SendFile(string fn)
    {
        Debug.Log("Time: " + Time.realtimeSinceStartup);
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

        GetFile();
    }

    /// <summary>
    /// Gets file from server.
    /// Called after sending one.
    /// </summary>
    public void GetFile()
    {
        Debug.Log("Getting File...");
        byte[] clientData = new byte[1024 * 5000];
        int receivedBytesLen = clientSocket.Receive(clientData);
        int fileNameLen = BitConverter.ToInt32(clientData, 0);
        string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

        fileName = fileDirectory + "//" + Path.GetFileName(fileName);

        Debug.Log(Application.persistentDataPath);

        //Write it
        BinaryWriter bWrite = new BinaryWriter(File.Open(fileName, FileMode.Create));
        bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
        bWrite.Close();
        Debug.Log($"Saved File At {fileName}");

        //Close Socket
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        clientSocket = null;

        //Spawn it
        UnityMainThreadDispatcher.Instance().Enqueue(() => new OBJLoader().Load(fileName));

        Debug.Log("Time: " + Time.realtimeSinceStartup);
    }
}
