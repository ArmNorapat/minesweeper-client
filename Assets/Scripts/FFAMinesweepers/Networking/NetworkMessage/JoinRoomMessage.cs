using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class JoinRoomMessage
    {
        public int roomID;

        public static JoinRoomMessage Parse(string[] parameters)
        {
            try
            {
                return new JoinRoomMessage()
                {
                    roomID = int.Parse(parameters[0])
                };
            }
            catch (Exception e)
            {
                throw new Exception("[JoinRoomMessage] " + e);
            }
        }

        public string Serialize()
        {
            return roomID.ToString();
        }

    }
}
