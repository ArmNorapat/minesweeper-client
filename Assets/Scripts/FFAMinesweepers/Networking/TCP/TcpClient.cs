using System;
using System.Net.Sockets;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;

namespace TrueAxion.FFAMinesweepers.Networking.TCP
{
    public class TcpClient : IDisposable
    {
        private TcpConnection tcpConnection;
        private string ipAddress;
        private int port;

        public TcpClient(string ipAddress, int port)
        {
            tcpConnection = new TcpConnection();
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public NetworkStream Start()
        {
            Console.WriteLine("[TCP Client] Start running.");
            return tcpConnection.Connect(ipAddress, port);
        }

        public void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Message can't empty or be null.");
            }

            tcpConnection.SendMessage(message);
        }

        public void Dispose()
        {
            Console.WriteLine("[TCP Client] Stop running.");
            tcpConnection.Dispose();
        }
    }
}