using TrueAxion.FFAMinesweepers.Event;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public class PlayerNameTextHandler : StringTextPlayerObserver
    {
        protected override EventAction<string> GetEvent()
        {
            return mineSweeperPlayer.PlayerNameUpdated;
        }
    }
}