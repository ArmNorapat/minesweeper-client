using System;
using System.Linq;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class TriggerSurroundCellMessage
    {
        public int PlayerId;
        public int CenterCellId;

        public static TriggerSurroundCellMessage Parse(string[] parameters)
        {
            return new TriggerSurroundCellMessage()
            {
                PlayerId = int.Parse(parameters[0]),
                CenterCellId = int.Parse(parameters[1]),
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.TriggerSurroundCell}|{PlayerId}|{CenterCellId}";
        }
    }
}