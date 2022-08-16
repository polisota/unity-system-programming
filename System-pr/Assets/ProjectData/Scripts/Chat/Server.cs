using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;
    private int port = 5805;
    private int hostID;
    private int reliableChannel;

    private bool isStarted = false;
    private byte error;    

    //List<int> connectionIDs = new List<int>();
    //Dictionary<int, string> connects = new Dictionary<int, string>(); //
    Dictionary<int, string> idName = new Dictionary<int, string>(); //
    Dictionary<int, bool> nameEntered = new Dictionary<int, bool>(); //

    public void StartServer()
    {
        GlobalConfig g = new GlobalConfig(); //
        //g.ReactorModel = ReactorModel.SelectReactor; //

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
    
    void Update()
    {
        if (!isStarted) return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        
        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    //connectionIDs.Add(connectionId);
                    nameEntered.Add(connectionId, false); //
                    SendMessageToAll($"Player {connectionId} has connected.");
                    Debug.Log($"Player {connectionId} has connected.");
                    break;

                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);

                    if (nameEntered[connectionId] == false) //
                    {
                        nameEntered[connectionId] = true; //
                        idName.Add(connectionId, message); //
                        SendMessageToAll($"{idName[connectionId]} has connected."); //
                    }
                    else
                    {
                        SendMessageToAll($"{idName[connectionId]}: {message}");
                        Debug.Log($"{idName[connectionId]}: {message}");
                    }                

                    //SendMessageToAll($"Player {connectionId}: {message}");
                    //Debug.Log($"Player {connectionId}: {message}");

                    break;

                case NetworkEventType.DisconnectEvent:
                    //connectionIDs.Remove(connectionId);
                    idName.Remove(connectionId); //
                    SendMessageToAll($"{idName[connectionId]} has disconnected."); //
                    Debug.Log($"{idName[connectionId]} has disconnected."); //

                    //SendMessageToAll($"Player {connectionId} has disconnected.");
                    //Debug.Log($"Player {connectionId} has disconnected.");

                    break;

                case NetworkEventType.BroadcastEvent:
                    break;
            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    public void SendMessageToAll(string message)
    {
        for (int i = 0; i<idName.Count; i++) //
        {
            SendMessage(message, idName[i]); //
        }
    }

    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok) 
            Debug.Log((NetworkError)error);
    }
}