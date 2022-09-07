using Characters;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private string _playerName;
        [SerializeField] private TMP_InputField _login;
        Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            //player.GetComponent<ShipController>().PlayerName = _playerName;
            player.GetComponent<ShipController>().PlayerName = _login.text;
            _players.Add(conn.connectionId, player.GetComponent<ShipController>());
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler(100, ReceiveName);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            MessageLogin mess = new MessageLogin();
            mess.login = _login.text;
            conn.Send(100, mess);
        }

        public void ReceiveName(NetworkMessage networkMessage)
        {
            _players[networkMessage.conn.connectionId].PlayerName = networkMessage.reader.ReadString();
            _players[networkMessage.conn.connectionId].gameObject.name = _players[networkMessage.conn.connectionId].PlayerName;
            Debug.Log(_players[networkMessage.conn.connectionId]);

        }

        public class MessageLogin : MessageBase
        {
            public string login;

            public override void Deserialize(NetworkReader reader)
            {
                login = reader.ReadString();
            }

            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(login);
            }
        }

    }
}
