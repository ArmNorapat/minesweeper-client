using TrueAxion.FFAMinesweepers.Gameplay;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    /// <summary>
    /// This text will observe the gameplayController
    /// </summary>
    public abstract class CountingTextGameplayObserver : CountingTextObserver
    {
        [SerializeField]
        protected GameplayController gameplayController = default;
    }
}