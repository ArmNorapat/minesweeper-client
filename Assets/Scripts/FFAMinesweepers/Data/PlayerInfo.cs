namespace TrueAxion.FFAMinesweepers.Data
{
    public struct PlayerInfo
    {
        public int PlayerId;
        public string PlayerName;

        public PlayerInfo(int playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
        }
    }
}
