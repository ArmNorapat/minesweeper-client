using TrueAxion.FFAMinesweepers.Player;
using TrueAxion.FFAMinesweepers.UI.TextObserver;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public abstract class StringTextPlayerObserver : StringTextObserver
    {
        [SerializeField]
        protected MineSweeperPlayer mineSweeperPlayer = default;
    }
}