using System.Collections;
using System.Collections.Generic;
using TrueAxion.FFAMinesweepers.Player;
using TrueAxion.FFAMinesweepers.UI.TextObserver;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    public abstract class CountingTextPlayerObserver : CountingTextObserver
    {
        [SerializeField]
        protected MineSweeperPlayer mineSweeperPlayer;
    }
}