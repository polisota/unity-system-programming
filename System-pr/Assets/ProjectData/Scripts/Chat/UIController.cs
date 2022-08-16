using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button buttonStartServer;
    [SerializeField] private Button buttonShutDownServer;
    [SerializeField] private Button buttonConnectClient;
    [SerializeField] private Button buttonDisconnectClient;
    [SerializeField] private Button buttonSendMessage;

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField nameField; //

    [SerializeField] private TextField textField;

    [SerializeField] private Server server;
    [SerializeField] private Client client;

    private void Start()
    {
        buttonStartServer.onClick.AddListener(() => StartServer());
        buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
        buttonConnectClient.onClick.AddListener(() => Connect(nameField.text)); //
        buttonDisconnectClient.onClick.AddListener(() => Disconnect());
        buttonSendMessage.onClick.AddListener(() => SendMessage());
        nameField.onEndEdit.AddListener((text) => EnableInteraction()); //
        client.onMessageReceive += ReceiveMessage;
    }

    private void EnableInteraction()//
    {
        buttonConnectClient.interactable = true; //
    }

    private void StartServer()
    {
        server.StartServer();
    }

    private void ShutDownServer()
    {
        server.ShutDownServer();
    }

    private void Connect(string message) //
    {
        client.Connect(message); //
    }

    private void Disconnect()
    {
        client.Disconnect();
    }

    private void SendMessage()
    {
        client.SendMessage(inputField.text);
        inputField.text = "";
    }

    public void ReceiveMessage(object message)
    {
        textField.ReceiveMessage(message);
    }
} 