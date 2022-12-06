using System;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class ReadyToStartMessage
    {
        public bool IsReadyToStart => isReadyToStart == 1 ? true : false;

        private int isReadyToStart;

        public static ReadyToStartMessage Parse(string[] parameters)
        {
            return new ReadyToStartMessage()
            {
                isReadyToStart = int.Parse(parameters[0])
            };
        }

        public string Serialize()
        {
            return IsReadyToStart.ToString();
        }
    }
}
