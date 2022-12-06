using System;
using TrueAxion.FFAMinesweepers.CellScripts;
using TrueAxion.FFAMinesweepers.Event;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.Player;
using TrueAxion.FFAMinesweepers.UI;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.Gameplay
{
    public class GameplayController : MonoBehaviour
    {
        public EventAction<int> FlagCountUpdate = new EventAction<int>();
        public EventAction<int> CurrentTimeUpdate = new EventAction<int>();
        public EventAction<int> CurrentScoreUpdate = new EventAction<int>();
        public event Action StartGame;
        public event Action GameOver;

        public bool IsGameEnd { get; private set; }

        [SerializeField]
        private CellManager cellManager = default;

        [SerializeField]
        private PlayersManager playersManager = default;

        [SerializeField]
        private CountdownController countdownController = default;

        private const int maxTimeCount = 999;

        private const string hostRestartGameMessage = "Press button to restart game.";
        private const string clientWaitForHostRestartMessage = "Waiting for host to restart.";
        private const string restartGameButtonName = "Restart";
        private const string clientTesterName = "Tester";

        private float currentTime;
        private int flagCount;
        private int triggeredCell;

        public void IncreaseFlagAmount()
        {
            SetFlagCount(flagCount + 1);
        }

        public void DecreaseFlagAmount()
        {
            SetFlagCount(flagCount - 1);
        }

        public void IncreaseTriggeredCellAmount()
        {
            triggeredCell++;

            if (IsAllNonBombCellTriggered())
            {
                OnGameOver();
            }
        }

        private void OnGameOver()
        {
            GameOver?.Invoke();
            IsGameEnd = true;

            if (playersManager.IsLocalPlayerIsHost)
            {
                PopupManager.Instance.ShowPopup(GetEndGamePopupMessage(),
                    restartGameButtonName, SendResetGameToNetwork);
            }
            else
            {
                PopupManager.Instance.ShowNonButtonPopup(GetEndGamePopupMessage());
            }
        }

        private void SendResetGameToNetwork()
        {
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.ResetGame, LocalClientHandler.Instance.LocalClientPlayerId);
        }

        private string GetEndGamePopupMessage()
        {
            var summary = playersManager.GetGameSummary();

            if (playersManager.IsLocalPlayerIsHost)
            {
                return $"{summary.GetSummaryMessage()}\n{hostRestartGameMessage}";
            }
            else
            {
                return $"{summary.GetSummaryMessage()}\n{clientWaitForHostRestartMessage}";
            }
        }

        private void SetFlagCount(int value)
        {
            flagCount = value;
            FlagCountUpdate.FireEvent(flagCount);
        }

        private void SetCurrentTime(float value)
        {
            currentTime = value;
            CurrentTimeUpdate.FireEvent(Mathf.RoundToInt(currentTime));
        }

        private bool IsAllNonBombCellTriggered()
        {
            return triggeredCell >= cellManager.NonBombCellCount();
        }

        private void Start()
        {
            SetupEvent();
            IsGameEnd = true;
        }

        private void OnRestartGame(GetSeedNumberMessage getSeedNumberMessage)
        {
            StartGame?.Invoke();

            SetFlagCount(CellManager.BombAmount);
            SetCurrentTime(0);

            triggeredCell = 0;
        }

        private void OnCountdownFinish()
        {
            IsGameEnd = false;
        }

        private void SendStartGameToNetwork()
        {
            //TODO : Join room in the lobby scene instead
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.JoinRoom, LocalClientHandler.Instance.LocalClientPlayerId, clientTesterName);
        }

        private void Update()
        {
            if (currentTime < maxTimeCount && !IsGameEnd)
            {
                var timer = currentTime + Time.deltaTime;
                SetCurrentTime(timer);
            }
        }

        private void SetupEvent()
        {
            NetworkMessageHandler.Instance.OnGetSeedNumberMessageReceived += OnRestartGame;
            playersManager.AllPlayersDead += OnGameOver;
            countdownController.CountdownFinished += OnCountdownFinish;
        }

        private void CleanUp()
        {
            if (NetworkMessageHandler.Instance != null)
            {
                NetworkMessageHandler.Instance.OnGetSeedNumberMessageReceived -= OnRestartGame;
            }

            if (playersManager != null)
            {
                playersManager.AllPlayersDead -= OnGameOver;
            }

            if (countdownController != null)
            {
                countdownController.CountdownFinished -= OnCountdownFinish;
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}