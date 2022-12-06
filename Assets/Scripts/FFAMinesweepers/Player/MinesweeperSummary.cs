using System.Linq;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Player
{
    public class MinesweeperSummary
    {
        private const int singleWinnerAmount = 1;

        //{0} is player color, {1} is player name
        private const string announceSingleWinnerMessage = "<#{0}> The winner is {1} </color>";
        private const string allPlayersDrawMessage = "All players draw!";
        private const string drawMessage = "You draw!";
        private const string loseMessage = "You lose!";

        public int BestHighScore { get; private set; }
        public int PlayerAmount { get; private set; }
        public MineSweeperPlayer LocalPlayer { get; private set; }
        public MineSweeperPlayer[] Players { get; private set; }

        public MinesweeperSummary(MineSweeperPlayer localPlayer, MineSweeperPlayer[] mineSweeperPlayers, int bestHighScore)
        {
            BestHighScore = bestHighScore;
            PlayerAmount = mineSweeperPlayers.Length;
            LocalPlayer = localPlayer;
            Players = mineSweeperPlayers.ToArray();
        }

        public string GetSummaryMessage()
        {
            var winnerAmount = CountPlayerWithHighScore();

            if (winnerAmount == singleWinnerAmount)
            {
                var winner = GetWinner();
                return string.Format(announceSingleWinnerMessage, ColorUtility.ToHtmlStringRGB(winner.PlayerColor), winner.PlayerName);
            }
            else if (winnerAmount == PlayerAmount)
            {
                return allPlayersDrawMessage;
            }
            else
            {
                if (LocalPlayer.CurrentScore == BestHighScore)
                {
                    return drawMessage;
                }
                else
                {
                    return loseMessage;
                }
            }
        }

        private int CountPlayerWithHighScore()
        {
            return Players.Where(playerInfo => playerInfo.CurrentScore == BestHighScore).ToArray().Length;
        }

        private MineSweeperPlayer GetWinner()
        {
            return Players.Where(playerInfo => playerInfo.CurrentScore == BestHighScore).First();
        }
    }
}