using System;
using System.Linq;
using TrueAxion.FFAMinesweepers.Threading;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class NetworkMessageHandler : MonoSingleton<NetworkMessageHandler>
    {
        /// <summary>
        /// We use this char to separate NetworkAction and Parameters. 
        /// </summary>
        public const char ParameterSeparator = '|';
        public const char ArraySeperator = ',';

        public Action<InstantiationMessage> OnInstantiationMessageReceived;
        public Action<GetNetworkIdMessage> OnGetNetworkIdMessageReceived;
        public Action<JoinRoomMessage> OnJoinRoomMessageReceived;
        public Action<RoomUpdateMessage> OnRoomUpdateMessageReceived;
        public Action<TriggerCellMessage> OnTriggerCellMessageReceived;
        public Action<GetSeedNumberMessage> OnGetSeedNumberMessageReceived;
        public Action<FlagCellMessage> OnFlagCellMessageReceived;
        public Action<TriggerSurroundCellMessage> OnTriggerSurroundCellMessageReceived;
        public Action<StartGameMessage> OnStartGameMessageReceived;
        public Action<GetCountdownTimeMessage> OnGetCountdownTimeMessageReceived;
        public Action<ReadyToPlayMessage> OnReadyToPlayMessageReceived;
        public Action<ReadyToStartMessage> OnReadyToStartMessageReceived;
        public Action<CancelReadyToPlayMessage> OnCancelReadyToPlayMessageReceived;

        public static int[] SplitParameterToArrayInt(string parameter)
        {
            return SplitParameterToArrayString(parameter).Select(int.Parse).ToArray();
        }

        public static string[] SplitParameterToArrayString(string parameter)
        {
            return parameter.Split(ArraySeperator).ToArray();
        }

        public void OnMessageReceived(string message)
        {
            JobManager.Instance.AddJob(() =>
            {
                try
                {
                    var parameters = SplitParametersAndAction(message, out NetworkAction targetNetworkAction);

                    switch (targetNetworkAction)
                    {
                        case NetworkAction.Instantiate:
                            OnInstantiationMessageReceived?.Invoke(InstantiationMessage.Parse(parameters));
                            break;

                        case NetworkAction.JoinRoom:
                            OnJoinRoomMessageReceived?.Invoke(JoinRoomMessage.Parse(parameters));
                            break;

                        case NetworkAction.UpdateRoom:
                            OnRoomUpdateMessageReceived?.Invoke(RoomUpdateMessage.Parse(parameters));
                            break;

                        case NetworkAction.GetNetworkId:
                            OnGetNetworkIdMessageReceived?.Invoke(GetNetworkIdMessage.Parse(parameters));
                            break;

                        case NetworkAction.ReadyToPlay:
                            OnReadyToPlayMessageReceived?.Invoke(ReadyToPlayMessage.Parse(parameters));
                            break;

                        case NetworkAction.CancelReadyToPlay:
                            OnCancelReadyToPlayMessageReceived?.Invoke(CancelReadyToPlayMessage.Parse(parameters));
                            break;

                        case NetworkAction.ReadyToStart:
                            OnReadyToStartMessageReceived?.Invoke(ReadyToStartMessage.Parse(parameters));
                            break;

                        case NetworkAction.StartGame:
                            OnStartGameMessageReceived?.Invoke(StartGameMessage.Parse(parameters));
                            break;

                        case NetworkAction.GetSeedNumber:
                            OnGetSeedNumberMessageReceived?.Invoke(GetSeedNumberMessage.Parse(parameters));
                            break;

                        case NetworkAction.TriggerCell:
                            OnTriggerCellMessageReceived?.Invoke(TriggerCellMessage.Parse(parameters));
                            break;

                        case NetworkAction.FlagCell:
                            OnFlagCellMessageReceived?.Invoke(FlagCellMessage.Parse(parameters));
                            break;

                        case NetworkAction.GetCountDownTime:
                            OnGetCountdownTimeMessageReceived?.Invoke(GetCountdownTimeMessage.Parse(parameters));
                            break;

                        case NetworkAction.TriggerSurroundCell:
                            OnTriggerSurroundCellMessageReceived?.Invoke(TriggerSurroundCellMessage.Parse(parameters));
                            break;

                        default:
                            throw new Exception("Couldn't find network action. From message : " + message);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("[NetworkMessage : " + message + " has error] => " + e);
                }
            });
        }

        /// <summary>
        /// Split network message to parameters and network action.
        /// </summary>
        /// <param name="message">Message from network.</param>
        /// <param name="targetNetworkAction">Define action of this message.</param>
        /// <returns>Return parameters from message.</returns>
        private string[] SplitParametersAndAction(string message, out NetworkAction targetNetworkAction)
        {
            var objects = message.Split(ParameterSeparator);

            if (int.TryParse(objects[0], out int actionId))
            {
                targetNetworkAction = (NetworkAction) actionId;

                return objects.Skip(1).ToArray();
            }
            else
            {
                throw new Exception($"Cannot parse action from message \"{message}\"");
            }
        }
    }
}