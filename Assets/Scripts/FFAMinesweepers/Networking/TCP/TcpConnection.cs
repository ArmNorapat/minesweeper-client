using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.Threading;
using TrueAxion.FFAMinesweepers.UI;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrueAxion.FFAMinesweepers.Networking.TCP
{
    public class TcpConnection : IDisposable
    {
        /// <summary>
        /// Some message may have more than 1 action with it. So, we have to separate them.
        /// </summary>
        private const char actionSeperator = '_';
        private const string debugReceiveMessageFormat = "[TCP Client] Action: {0} => Received: {1}";
        private const string debugSentMessageFormat = "[TCP Client] Action: {0} => Sent: {1}";
        private const string connectionLostMessage = "Connection lost!";
        private const int responseBytes = 256;
        private const int actionIndex = 0;

        private System.Net.Sockets.TcpClient client;
        private NetworkStream stream;
        private Thread receiveMessageThread;

        public NetworkStream Connect(string server, Int32 port)
        {
            try
            {
                client = new System.Net.Sockets.TcpClient(server, port);
                stream = client.GetStream();

                receiveMessageThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveMessageThread.IsBackground = true;
                receiveMessageThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }

            return stream;
        }

        public void SendMessage(string message)
        {
            if (stream.CanWrite)
            {
                // Translate the passed message into UTF8 and store it as a Byte array.
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);
                Debug.LogFormat(debugSentMessageFormat, GetActionFromString(message).ToString(""), message);
            }
            else
            {
                AlertConnectionLost();
            }
        }

        public void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    // Buffer to store the response bytes.
                    Byte[] bytes = new Byte[responseBytes];
                    string responseData = null;
                    int numberOfBytesRead = 0;

                    if (stream.CanRead)
                    {
                        do
                        {
                            // Translate data bytes to a UTF8 string.
                            numberOfBytesRead = stream.Read(bytes, 0, bytes.Length);
                            responseData = responseData + Encoding.UTF8.GetString(bytes, 0, numberOfBytesRead);
                        }
                        while (stream.DataAvailable);

                        if (responseData != string.Empty)
                        {
                            //Split actions.
                            var actions = responseData.Split(actionSeperator);

                            foreach (string action in actions)
                            {
                                if (action != string.Empty)
                                {
                                    NetworkMessageHandler.Instance.OnMessageReceived(action);
                                    Debug.LogFormat(debugReceiveMessageFormat, GetActionFromString(action).ToString(""), responseData);
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (IOException)
            {
                Debug.Log($"[TCP Client] Server was forcibly closed");
            }
            catch (Exception e)
            {
                Debug.Log($"Exception: {e}");
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            AlertConnectionLost();

            if (stream != null)
            {
                stream.Close();
                client.Close();
                receiveMessageThread.Interrupt();
            }
        }

        private void AlertConnectionLost()
        {
            JobManager.Instance.AddJob(() =>
            {
                PopupManager.Instance.ShowPopup(connectionLostMessage, () =>
                {
                    SceneManager.LoadScene(SceneList.LobbyScene);
                });
            });
        }

        private NetworkAction GetActionFromString(string responseData)
        {
            if (!int.TryParse(responseData.Split(NetworkMessageHandler.ParameterSeparator) [actionIndex], out int action))
            {
                action = 0;
            }

            return (NetworkAction) action;
        }
    }
}