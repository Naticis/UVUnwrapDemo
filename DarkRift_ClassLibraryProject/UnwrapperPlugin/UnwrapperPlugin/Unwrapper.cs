using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnwrapperPlugin
{
    public class Unwrapper : Plugin
    {
        public override bool ThreadSafe => false;

        public override Version Version => new Version(0, 0, 1);

        public override Command[] Commands => base.Commands;

        public Unwrapper(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += ClientManager_ClientConnected;
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;


            Task task = Task.Run(() => TCPListener.Start());
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"Client {e.Client.ID} disconnected from plugin.");
        }

        private void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine($"Client {e.Client.ID} connected to plugin.");
            e.Client.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch(e.Tag)
            {
                case Tags.SendObj:
                    {
                        //SendObj_Handler(e);
                        break;
                    }                    
            }
        }
        private void SendObj_Handler(MessageReceivedEventArgs e)
        {
            Console.WriteLine($"Received message of SendObj");
            using (Message msg = e.GetMessage())
            {
                using (DarkRiftReader reader = msg.GetReader())
                {
                    byte[] objAsBytes = reader.ReadBytes();
                    string objAsString = Encoding.ASCII.GetString(objAsBytes);
                    SaveStringAsObj(objAsString, AppDomain.CurrentDomain.BaseDirectory + "\\stringAsObj.obj");
                }
            }
        }

        private void SaveStringAsObj(string objAsString, string fileName)
        {
            Console.WriteLine($"Attempting to save obj as file");
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(objAsString);
            }
            Console.WriteLine($"Saved string as obj under {fileName}");
        }

        
    }
}
