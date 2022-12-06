using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class InstantiationMessage
    {
        public int PlayerId;
        public string ObjectPath;
        public string ParentName;

        public static InstantiationMessage Parse(string[] parameters)
        {
            return new InstantiationMessage()
            {
                PlayerId = int.Parse(parameters[0]),
                ObjectPath = parameters[1],
                ParentName = parameters[2]
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.Instantiate}|{ObjectPath}";
        }
    }
}