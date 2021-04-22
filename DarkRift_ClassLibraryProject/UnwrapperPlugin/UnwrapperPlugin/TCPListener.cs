using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UnwrapperPlugin
{
    public static class TCPListener
    {
        private static Socket clientSocket;
        public static void Start()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            Console.WriteLine("Starting TCP Listener");

            IPEndPoint ipEnd = new IPEndPoint(ipAddress, 3004);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            serverSocket.Bind(ipEnd);

            int count = 0;

            serverSocket.Listen(3004);

            Console.WriteLine("Server started...");
            while (true)
            {
                count++;

                clientSocket = serverSocket.Accept();
                Console.WriteLine($"Client Connected. Total :  {count}");

                new Thread(delegate ()
                {
                    GetFile(clientSocket);
                }).Start();
            }
        }
        public static void GetFile(Socket clientSocket)
        {
            Console.WriteLine("Getting File...");
            byte[] clientData = new byte[1024 * 5000];
            int receivedBytesLen = clientSocket.Receive(clientData);
            int fileNameLen = BitConverter.ToInt32(clientData, 0);
            string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

            fileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(fileName);

            BinaryWriter bWrite = new BinaryWriter(File.Open(fileName, FileMode.Create));
            bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
            bWrite.Close();
            Console.WriteLine($"Saved File At {fileName}");
            Unwrap();
        }

        public static void Unwrap()
        {
            Console.WriteLine("Trying to unwrap");
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + "blender --background --python unwrap.py");
            ProcessInfo.CreateNoWindow = false;
            ProcessInfo.UseShellExecute = true;

            Process = new Process();
            Process.EnableRaisingEvents = true;      
            Process.StartInfo = ProcessInfo;
            Process.Exited += Process_Exited;
            Process.Start();

            Console.WriteLine("Unwraped");
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("PROCESS ENDED");
            string fileToSend = AppDomain.CurrentDomain.BaseDirectory + "\\Unwrapped.obj";
            SendFile(fileToSend);
        }
        private static void SendFile(string fn)
        {
            Console.WriteLine($"Trying to send file {fn}");
            //IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            //IPEndPoint ipEnd = new IPEndPoint(ipAddress, 3005);
            //Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            string fileName = fn;// "c:\\filetosend.txt";
            byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
            byte[] fileData = File.ReadAllBytes(fileName);
            byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];

            fileNameLen.CopyTo(clientData, 0);
            fileNameByte.CopyTo(clientData, 4);
            fileData.CopyTo(clientData, 4 + fileNameByte.Length);
        //    clientSocket.Connect(ipEnd);
            clientSocket.Send(clientData);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Console.WriteLine($"Sent file: {fn}");

            //[0]filenamelen[4]filenamebyte[*]filedata     
        }
    }
}
