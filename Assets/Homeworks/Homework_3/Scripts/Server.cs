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

        Dictionary<int, string> players = new Dictionary<int, string>();

        public void StartServer()
        {
            NetworkTransport.Init();
            ConnectionConfig cc = new ConnectionConfig();
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
                        SendToChatAndConsole($"{players[connectionId]} has connected.");
                        break;
                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);

                        if (IsRenaming(message))
                        {
                            string name = MessageIDs.CommandToMessage(message, MessageIDs.RENAMING_COMMAND);
                            
                            RenamePlayer(connectionId, name);
                            SendMessage($"Your name is {name}", connectionId);
                            break;
                        }
                        SendToChatAndConsole($"{players[connectionId]}: {message}");                      
                        break;
                    case NetworkEventType.DisconnectEvent:
                        connectionIDs.Remove(connectionId);
                        SendToChatAndConsole($"{players[connectionId]} has disconnected.");                 
                        RemovePlayer(connectionId);
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out error);
            }               
        }

        private void SendToChatAndConsole(string message)
        {
            SendMessageToAll(message);
            Debug.Log(message);
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

        private bool IsRenaming(string message)
        {
            for(int i = 0;i < MessageIDs.RENAMING_COMMAND.Length ;i++) 
            {
                if (message[i] == MessageIDs.RENAMING_COMMAND[i])
                    continue;
                return false;
            }
            return true;
        }

        private void AddNewPlayer(int connectionId) => players.Add(connectionId, DEFAULT_NAME + connectionId);

        private void RemovePlayer(int connectionId) => players.Remove(connectionId);

        private void RenamePlayer(int connectionId, string newName) => players[connectionId] = newName;
    }
}       
