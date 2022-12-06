using System.Collections;
using System.Collections.Generic;
using TrueAxion.FFAMinesweepers.Event;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public class CountdownTextHandler : CountingTextCountdownObserver
    {
        protected override EventAction<int> GetEvent()
        {
            return countdownController.CountdownNumberChanged;
        }
    }
}