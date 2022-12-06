using System;
using System.Net.Sockets;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.Networking.TCP;
using TrueAxion.FFAMinesweepers.UI;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using TcpClient = TrueAxion.FFAMinesweepers.Networking.TCP.TcpClient;

namespace TrueAxion.FFAMinesweepers.Networking
{
    public class LocalClientHandler : MonoSingleton<LocalClientHandler>
    {
        public const string PlayerPrefServerIpKey = "serverIp";
        public const string PlayerPrefsPortKey = "serverPort";
        public int LocalClientPlayerId { get; private set; }
        public string LocalClientPlayerName { get; private set; }
        public bool IsClientExit { get { return clientNetworkStream != null && clientNetworkStream.CanWrite; } }

        private NetworkStream clientNetworkStream;
        private TcpClient localTcpClient;

        public bool ConnectToServer(string serverIp, int port)
        {
            localTcpClient = new TcpClient(serverIp, port);
            clientNetworkStream = localTcpClient.Start();

            PlayerPrefs.SetString(PlayerPrefServerIpKey, serverIp);
            PlayerPrefs.SetInt(PlayerPrefsPortKey, port);

            return IsClientExit;
        }

        public void SendMessageToNetwork(NetworkAction action, int playerId, params object[] parameters)
        {
            if (IsClientExit)
            {
                string message = $"{(int)action}{NetworkMessageHandler.ParameterSeparator}{playerId}";

                foreach (object param in parameters)
                {
                    message += $"{NetworkMessageHandler.ParameterSeparator}{param.ToString()}";
                }

                localTcpClient.SendMessage(message);
            }
        }

        public void NetworkInstantiate(string objectPath, GameObject parentObj)
        {
            SendMessageToNetwork(Networking.NetworkAction.Instantiate, LocalClientPlayerId, objectPath, parentObj.name);
        }

        private void OnGetNetworkId(GetNetworkIdMessage getNetworkIdMessage)
        {
            LocalClientPlayerId = getNetworkIdMessage.ClientNetworkId;
        }

        private void OnInstantiateFromNetwork(InstantiationMessage instantiationMessage)
        {
            try
            {
                var playerId = instantiationMessage.PlayerId;
                var objectPath = instantiationMessage.ObjectPath;
                var parentName = instantiationMessage.ParentName;

                var prefab = Resources.Load(objectPath);
                var parent = GameObject.Find(parentName);

                Instantiate(prefab, parent.transform);
            }
            catch (Exception e)
            {
                Debug.Log("[Network instantiate] Can't instantiate object from network : " + e);
            }
        }

        private void Start()
        {
            SetupEvent();
        }

        private void OnDestroy()
        {
            if (localTcpClient != null)
            {
                CleanUp();
                localTcpClient.Dispose();
            }
        }

        private void SetupEvent()
        {
            NetworkMessageHandler.Instance.OnGetNetworkIdMessageReceived += OnGetNetworkId;
            NetworkMessageHandler.Instance.OnInstantiationMessageReceived += OnInstantiateFromNetwork;
        }

        private void CleanUp()
        {
            if (NetworkMessageHandler.Instance != null)
            {
                NetworkMessageHandler.Instance.OnGetNetworkIdMessageReceived -= OnGetNetworkId;
                NetworkMessageHandler.Instance.OnInstantiationMessageReceived -= OnInstantiateFromNetwork;
            }
        }
    }
}