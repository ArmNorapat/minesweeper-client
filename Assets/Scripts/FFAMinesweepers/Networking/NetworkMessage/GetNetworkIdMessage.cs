using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class GetNetworkIdMessage
    {
        public int ClientNetworkId;

        public static GetNetworkIdMessage Parse(string[] parameters)
        {
            return new GetNetworkIdMessage()
            {
                ClientNetworkId = int.Parse(parameters[0]),
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.GetNetworkId}|{ClientNetworkId}";
        }
    }
}