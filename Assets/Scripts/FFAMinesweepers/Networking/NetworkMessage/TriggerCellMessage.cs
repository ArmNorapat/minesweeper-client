using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class TriggerCellMessage
    {
        public int PlayerId;
        public int CellId;

        public static TriggerCellMessage Parse(string[] parameters)
        {
            return new TriggerCellMessage()
            {
                PlayerId = int.Parse(parameters[0]),
                CellId = int.Parse(parameters[1])
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.TriggerCell}|{PlayerId}|{CellId}";
        }
    }
}