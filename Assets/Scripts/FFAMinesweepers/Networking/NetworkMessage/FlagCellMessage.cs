using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class FlagCellMessage
    {
        public int PlayerId;
        public int CellId;

        public static FlagCellMessage Parse(string[] parameters)
        {
            return new FlagCellMessage()
            {
                PlayerId = int.Parse(parameters[0]),
                CellId = int.Parse(parameters[1])
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.FlagCell}|{PlayerId}|{CellId}";
        }
    }
}