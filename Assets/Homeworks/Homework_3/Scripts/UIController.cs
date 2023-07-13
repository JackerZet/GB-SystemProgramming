using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Homeworks.homework_3
{

    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private Button buttonStartServer;
        [SerializeField]
        private Button buttonShutDownServer;
        [SerializeField]
        private Button buttonConnectClient;
        [SerializeField]
        private Button buttonDisconnectClient;
        [SerializeField]
        private Button buttonSendMessage;
        [SerializeField]
        private Button buttonSendName;
        [SerializeField]
        private TMP_InputField inputMessageField;
        [SerializeField]
        private TMP_InputField inputNameField;
        [SerializeField]
        private TextField textField;
        [SerializeField]
        private Server server;
        [SerializeField]
        private Client client;
        private void Start()
        {
            buttonStartServer.onClick.AddListener(() => StartServer());
            buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
            buttonConnectClient.onClick.AddListener(() => Connect());
            buttonDisconnectClient.onClick.AddListener(() => Disconnect());
            buttonSendMessage.onClick.AddListener(() => SendMessage());
            buttonSendName.onClick.AddListener(() => SendName());
            client.onMessageReceive += ReceiveMessage;
        }
        private void OnDestroy()
        {
            buttonStartServer.onClick.RemoveAllListeners();
            buttonShutDownServer.onClick.RemoveAllListeners();
            buttonConnectClient.onClick.RemoveAllListeners();
            buttonDisconnectClient.onClick.RemoveAllListeners();
            buttonSendMessage.onClick.RemoveAllListeners();
            client.onMessageReceive -= ReceiveMessage;
        }
        private void StartServer()
        {
            server.StartServer();
        }
        private void ShutDownServer()
        {
            server.ShutDownServer();
        }
        private void Connect()
        {
            client.Connect();
        }
        private void Disconnect()
        {
            client.Disconnect();
        }
        private void SendMessage()
        {
            client.SendMessage(inputMessageField.text);
            inputMessageField.text = "";
        }
        private void SendName()
        {
            client.SendName(inputNameField.text);
        }
        public void ReceiveMessage(object message)
        {
            textField.ReceiveMessage(message);
        }
    }
}
