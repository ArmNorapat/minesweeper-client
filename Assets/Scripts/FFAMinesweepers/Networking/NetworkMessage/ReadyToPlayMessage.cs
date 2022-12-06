using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class ReadyToPlayMessage
    {
        public int playerID;

        public static ReadyToPlayMessage Parse(string[] parameters)
        {
            return new ReadyToPlayMessage()
            {
                playerID = int.Parse(parameters[0])
            };
        }

        public string Serialize()
        {
            return playerID.ToString();
        }
    }
}
