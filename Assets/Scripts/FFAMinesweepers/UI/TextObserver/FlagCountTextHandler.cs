using TrueAxion.FFAMinesweepers.Event;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public class FlagCountTextHandler : CountingTextGameplayObserver
    {
        protected override EventAction<int> GetEvent()
        {
            return gameplayController.FlagCountUpdate;
        }
    }
}