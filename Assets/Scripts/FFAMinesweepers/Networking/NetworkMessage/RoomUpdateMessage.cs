using System;
using System.Linq;
using TrueAxion.FFAMinesweepers.Data;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class RoomUpdateMessage
    {
        public const string playerNameFormat = "Player {0}";

        public int PlayerAmount;
        public int MasterClientId;
        public PlayerInfo[] playersInfo;

        public static RoomUpdateMessage Parse(string[] parameters)
        {
            var playersId = NetworkMessageHandler.SplitParameterToArrayInt(parameters[2]);
            var playersName = NetworkMessageHandler.SplitParameterToArrayString(parameters[3]);

            return new RoomUpdateMessage()
            {
                PlayerAmount = int.Parse(parameters[0]),
                MasterClientId = int.Parse(parameters[1]),
                playersInfo = GetPlayersInformation(playersId, playersName)
            };
        }

        public string Serialize()
        {
            var playersId = "";
            var playersName = "";

            foreach (PlayerInfo playerInfo in playersInfo)
            {
                playersId += playerInfo.PlayerId + NetworkMessageHandler.ArraySeperator;
                playersName += playerInfo.PlayerName + NetworkMessageHandler.ArraySeperator;
            }

            return $"{NetworkAction.UpdateRoom}|{playersId}|{playersName}";
        }

        private static PlayerInfo[] GetPlayersInformation(int[] playersId, string[] playersName)
        {
            var players = new PlayerInfo[playersId.Length];

            for (int i = 0; i < playersId.Length; i++)
            {
                players[i] = new PlayerInfo(playersId[i], playersName[i]);
            }

            return players;
        }
    }
}