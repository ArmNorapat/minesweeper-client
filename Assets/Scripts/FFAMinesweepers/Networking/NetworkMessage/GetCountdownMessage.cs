using System;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class GetCountdownTimeMessage
    {
        public int Timestamp;

        public static GetCountdownTimeMessage Parse(string[] parameters)
        {
            return new GetCountdownTimeMessage()
            {
                Timestamp = int.Parse(parameters[0]),
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.FlagCell}|{Timestamp}";
        }
    }
}