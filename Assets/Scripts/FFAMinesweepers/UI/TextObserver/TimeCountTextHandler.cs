using System;
using TrueAxion.FFAMinesweepers.Event;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public class TimeCountTextHandler : CountingTextGameplayObserver
    {
        protected override EventAction<int> GetEvent()
        {
            return gameplayController.CurrentTimeUpdate;
        }
    }
}