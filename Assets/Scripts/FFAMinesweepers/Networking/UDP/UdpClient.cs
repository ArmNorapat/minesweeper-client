using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Networking.UDP
{
    public class UdpClient : IDisposable
    {
        public struct UdpState
        {
            public System.Net.Sockets.UdpClient Client;
            public IPEndPoint EndPoint;
        }

        private Thread receiveMessageThread;
        private UdpState udpState;
        private System.Net.Sockets.UdpClient udpClient;

        public UdpClient(int port, string ipAddress)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            udpClient = new System.Net.Sockets.UdpClient(AddressFamily.InterNetwork);
            udpClient.Connect(ipEndPoint);

            udpState = new UdpState();
            udpState.Client = udpClient;
            udpState.EndPoint = ipEndPoint;
        }

        public void Start()
        {
            Console.WriteLine("[UDP Client] Start running.");

            receiveMessageThread = new Thread(new ThreadStart(() => StartReceiveNewMessage(udpState)));
            receiveMessageThread.IsBackground = true;

            receiveMessageThread.Start();
        }

        public void SendMessage(string newMessage)
        {
            if (newMessage == string.Empty)
            {
                newMessage = "PING";
            }

            Debug.Log($"[UDP Client] Sent: {newMessage}");

            byte[] byteSend = Encoding.UTF8.GetBytes(newMessage);

            try
            {
                udpClient.Send(byteSend, byteSend.Length);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void Dispose()
        {
            udpClient.Close();
            receiveMessageThread.Abort();
        }

        private void StartReceiveNewMessage(UdpState udpState)
        {
            udpClient.BeginReceive(OnMessageReceived, udpState);
        }

        private void OnMessageReceived(IAsyncResult asyncResult)
        {
            UdpState newUdpState = (UdpState)asyncResult.AsyncState;

            byte[] receiveBytes = newUdpState.Client.EndReceive(asyncResult, ref newUdpState.EndPoint);
            string receiveString = Encoding.UTF8.GetString(receiveBytes);

            Debug.Log($"[UDP Client] Received: {receiveString}");

            StartReceiveNewMessage(newUdpState);
        }
    }
}