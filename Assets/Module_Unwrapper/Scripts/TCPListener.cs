using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TCPListener :MonoBehaviour
{
    private string filePath;
    [RuntimeInitializeOnLoadMethod]
    private void Start()
    {
        Debug.Log("HELLO");
        filePath = Application.persistentDataPath;
        Task task = Task.Run(() => Initialize());
    }

    public void Initialize()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

        Debug.Log("Starting TCP Listener");

        IPEndPoint ipEnd = new IPEndPoint(ipAddress, 3005);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        serverSocket.Bind(ipEnd);

        int count = 0;

        serverSocket.Listen(3005);

        Debug.Log("Server started...");
        while (true)
        {
            count++;

            Socket clientSocket = serverSocket.Accept();
            Debug.Log($"Client Connected. Total :  {count}");

            new Thread(delegate ()
            {
                doChat(clientSocket, count.ToString());
            }).Start();
        }
    }
    public void doChat(Socket clientSocket, string n)
    {
        Debug.Log($"Getting File...{filePath}");
        byte[] clientData = new byte[1024 * 5000];
        int receivedBytesLen = clientSocket.Receive(clientData);
        int fileNameLen = BitConverter.ToInt32(clientData, 0);
        string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

        fileName = filePath + "\\" + Path.GetFileName(fileName);

        BinaryWriter bWrite = new BinaryWriter(File.Open(fileName, FileMode.Create));
        bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
        bWrite.Close();
        Debug.Log($"Saved File At {fileName}");
        UnityMainThreadDispatcher.Instance().Enqueue(() => Unwrapper.LoadObjFromFile(fileName));
    }
}
