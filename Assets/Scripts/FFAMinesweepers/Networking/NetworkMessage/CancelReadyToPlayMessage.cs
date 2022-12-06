using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class CancelReadyToPlayMessage
    {
        public int playerID;

        public static CancelReadyToPlayMessage Parse(string[] parameters)
        {
            return new CancelReadyToPlayMessage()
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
