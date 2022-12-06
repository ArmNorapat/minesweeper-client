namespace TrueAxion.FFAMinesweepers.Networking
{
    public enum NetworkAction
    {
        None,
        Instantiate,
        JoinRoom,
        UpdateRoom,
        GetNetworkId,
        ReadyToPlay,
        CancelReadyToPlay,
        ReadyToStart,
        StartGameRequest,
        StartGame,
        ResetGame,
        GetSeedNumber,
        ReadyToCountDown,
        GetCountDownTime,
        TriggerCell,
        FlagCell,
        TriggerSurroundCell,
        LeaveRoom,
        GameOver
    }
}