using System;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    //Assume get new seed is RestartGame.
    public class GetSeedNumberMessage
    {
        public int SeedNumber;

        public static GetSeedNumberMessage Parse(string[] parameters)
        {
            return new GetSeedNumberMessage()
            {
                SeedNumber = int.Parse(parameters[0]),
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.GetSeedNumber}|{SeedNumber}";
        }
    }
}