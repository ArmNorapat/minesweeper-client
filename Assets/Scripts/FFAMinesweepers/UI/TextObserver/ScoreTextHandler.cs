using TrueAxion.FFAMinesweepers.Event;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public class ScoreTextHandler : CountingTextPlayerObserver
    {
        protected override EventAction<int> GetEvent()
        {
            return mineSweeperPlayer.CurrentScoreUpdated;
        }
    }
}