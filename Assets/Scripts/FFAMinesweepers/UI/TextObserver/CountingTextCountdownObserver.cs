using TrueAxion.FFAMinesweepers.Gameplay;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public abstract class CountingTextCountdownObserver : CountingTextObserver
    {
        [SerializeField]
        protected CountdownController countdownController = default;
    }
}