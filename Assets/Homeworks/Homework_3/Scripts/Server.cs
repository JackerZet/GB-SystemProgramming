using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Homeworks.homework_3
{
    public class Server : MonoBehaviour
    {
        private const int MAX_CONNECTION = 10;
        private const string DEFAULT_NAME = "Player ";

        private int port = 6701;
        private int hostID;
        private int reliableChannel;
        private bool isStarted = false;
        private byte error;

        List<int> connectionIDs = new List<int>();

        Dictionary<int, string> playersNames = new Dictionary<int, string>();

        public void StartServer()
        {
            NetworkTransport.Init();
            ConnectionConfig cc = new ConnectionConfig();
            cc.ConnectTimeout = 100;
            //cc.DisconnectTimeout = 100;
            reliableChannel = cc.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
            hostID = NetworkTransport.AddHost(topology, port);
            isStarted = true;
        }

        public void ShutDownServer()
        {
            if (!isStarted) return;
            NetworkTransport.RemoveHost(hostID);
            NetworkTransport.Shutdown();
            isStarted = false;
        }

        private void Update()
        {
            if (!isStarted) return;
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
                        connectionIDs.Add(connectionId);
                        AddNewPlayer(connectionId);
                        SendMessageToAll($"{playersNames[connectionId]} has connected.");
                        Debug.Log($"{playersNames[connectionId]} has connected.");
                        break;
                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        SendMessageToAll($"{playersNames[connectionId]}: {message}");
                        Debug.Log($"{playersNames[connectionId]}: {message}");
                        if (playersNames[connectionId] == DEFAULT_NAME + connectionId)
                            RenamePlayer(connectionId, message);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        connectionIDs.Remove(connectionId);
                        RemovePlayer(connectionId);
                        SendMessageToAll($"{playersNames[connectionId]} has disconnected.");
                        Debug.Log($"{playersNames[connectionId]} has disconnected.");
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out error);
            }               
        }
        public void SendMessageToAll(string message)
        {
            for (int i = 0; i < connectionIDs.Count; i++)
            {
                SendMessage(message, connectionIDs[i]);
            }
        }

        public void SendMessage(string message, int connectionID)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
            if ((NetworkError)error != NetworkError.Ok) Debug.Log((NetworkError)error);
        }

        private void AddNewPlayer(int connectionId)
        {
            playersNames.Add(connectionId, DEFAULT_NAME + connectionId);
        }

        private void RemovePlayer(int connectionId)
        {
            playersNames.Remove(connectionId);
        }

        private void RenamePlayer(int connectionId, string newName)
        {
            playersNames[connectionId] = newName;
        }
    }
}       
