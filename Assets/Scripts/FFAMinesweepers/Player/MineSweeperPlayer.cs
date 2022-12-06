using System;
using TrueAxion.FFAMinesweepers.Event;
using TrueAxion.FFAMinesweepers.Gameplay;
using TrueAxion.FFAMinesweepers.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.Player
{
    public class MineSweeperPlayer : MonoBehaviour
    {
        public EventAction<int> CurrentScoreUpdated = new EventAction<int>();
        public EventAction<string> PlayerNameUpdated = new EventAction<string>();
        public event Action<PlayerState> PlayerStateChanged;
        public int CurrentScore { get; private set; }
        public int OwnerId { get; private set; }
        public Color PlayerColor { get; private set; }
        public string PlayerName { get; private set; }
        public int TotalScore { get { return hiddenScore + CurrentScore; } }
        public bool IsDead { get { return currentPlayerState == PlayerState.Lose; } }

        private const int hiddenScorePerBomb = 5;
        private const string scoreStringFormat = "000";

        [Tooltip("Visualize if this is mine object.")]
        [SerializeField]
        private GameObject localPlayerTextObj = default;

        [SerializeField]
        private Image playerBanner = default;

        private CellPointerManager cellPointerManager = default;
        private PlayerState currentPlayerState = PlayerState.Idle;
        private GameplayController gameplayController;
        private PlayersManager playersManager;
        private int hiddenScore;

        public void SetGameplayController(GameplayController gameplayController)
        {
            this.gameplayController = gameplayController;
        }

        public void SetPlayersManager(PlayersManager playersManager)
        {
            this.playersManager = playersManager;
        }

        public void SetCellPointerManager(CellPointerManager cellPointerManager)
        {
            this.cellPointerManager = cellPointerManager;
        }

        public void IncreaseScore()
        {
            SetCurrentScore(CurrentScore + 1);
        }

        public void SetPlayerName(string newName)
        {
            PlayerNameUpdated.FireEvent(newName);
            PlayerName = newName;
        }

        public void SetPlayerColor(Color color)
        {
            PlayerColor = color;
            playerBanner.color = color;
        }

        public void SetPlayerState(PlayerState playerState)
        {
            PlayerStateChanged?.Invoke(playerState);
            currentPlayerState = playerState;
        }

        public void SetOwnerId(int playerId)
        {
            OwnerId = playerId;
        }

        public void SetLocalPlayerTextActive(bool value)
        {
            localPlayerTextObj.SetActive(value);
        }

        public void IncreaseHiddenScoreOnFlagBombCell()
        {
            hiddenScore += hiddenScorePerBomb;
        }

        public void DecreaseHiddenScoreOnUnflagBombCell()
        {
            hiddenScore -= hiddenScorePerBomb;
        }

        public void SummaryPlayerScore()
        {
            SetCurrentScore(TotalScore);
        }

        private void OnPlayerInspectingSurroundCell(bool isInspect)
        {
            if (currentPlayerState == PlayerState.Lose || currentPlayerState == PlayerState.Win)
            {
                return;
            }

            if (isInspect)
            {
                SetPlayerState(PlayerState.Inspecting);
            }
            else
            {
                SetPlayerState(PlayerState.Idle);
            }
        }

        private void SetCurrentScore(int value)
        {
            CurrentScore = value;
            CurrentScoreUpdated.FireEvent(value);
        }

        private void ResetPlayer()
        {
            hiddenScore = 0;
            SetCurrentScore(0);
            SetPlayerState(PlayerState.Idle);
        }

        private void Start()
        {
            SetupEvent();
        }

        private void SetupEvent()
        {
            cellPointerManager.InspectingSurroundCell += OnPlayerInspectingSurroundCell;
            gameplayController.StartGame += ResetPlayer;
        }

        private void CleanUp()
        {
            if (cellPointerManager != null)
            {
                cellPointerManager.InspectingSurroundCell += OnPlayerInspectingSurroundCell;
            }
            if (gameplayController != null)
            {
                gameplayController.StartGame -= ResetPlayer;
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}