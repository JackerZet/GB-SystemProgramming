﻿using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Homeworks.homework_3
{ 
    public class Client : MonoBehaviour
    {
        public delegate void OnMessageReceive(object message);
        public event OnMessageReceive onMessageReceive;

        private const int MAX_CONNECTION = 10;
        private int port = 0;
        private int serverPort = 6701;
        private int hostID;
        private int reliableChannel;
        private byte error;
        private int connectionID = -1;

        private bool isConnected = false;

        public string inputName = string.Empty;
        public void Connect()
        {
            if (isConnected) return;
          
            NetworkTransport.Init();
            ConnectionConfig cc = new ConnectionConfig();
            
            reliableChannel = cc.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
            hostID = NetworkTransport.AddHost(topology, port);        
            connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", serverPort, 0, out error);
            if ((NetworkError)error == NetworkError.Ok)            
                isConnected = true;
            else           
                Debug.Log((NetworkError)error);            
        }
        public void Disconnect()
        {
            if (!isConnected) return;
            NetworkTransport.Disconnect(hostID, connectionID, out error);
            
            isConnected = false;
        }
        void Update()
        {
            if (!isConnected) return;
            int recHostId;
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out
            channelId, recBuffer, bufferSize, out dataSize, out error);

            while (recData != NetworkEventType.Nothing)
            {
                switch (recData)
                {
                    case NetworkEventType.Nothing:
                        break;

                    case NetworkEventType.ConnectEvent:
                        SendToChatAndConsole($"You have been connected to server.");
                        break;

                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);                       
                        SendToChatAndConsole(message);                        
                        break;

                    case NetworkEventType.DisconnectEvent:
                        if(this.connectionID != connectionId)
                            isConnected = false;  // Thunderstorm of all troubles
                        SendToChatAndConsole($"You have been disconnected from server.");                      
                        break;

                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out error);
            }
        }

        
        public void SendMessage(string message)
        {
            SendToServer(message);
        }

        public void SendName(string name) 
        {           
            SendToServer(MessageIDs.MessageToCommand(name, MessageIDs.RENAMING_COMMAND));
        }

        private void SendToServer(string message)
        {
            if(!isConnected) { return; }
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
            if ((NetworkError)error != NetworkError.Ok) Debug.Log((NetworkError)error);
        }

        private void SendToChatAndConsole(string message)
        {
            onMessageReceive?.Invoke(message);
            Debug.Log(message);
        }
    }
}

