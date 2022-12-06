using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueAxion.FFAMinesweepers.Gameplay;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.UI;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrueAxion.FFAMinesweepers.Player
{
    public class PlayersManager : MonoBehaviour
    {
        public event Action AllPlayersDead;

        public bool IsLocalPlayerDead { get; private set; }
        public bool IsLocalPlayerIsHost { get { return localPlayerId == HostId; } }
        public int HostId { get; private set; }

        //{0} Color, {1} Player name.
        private const string playerNameWithColorFormat = "<#{0}>{1}</color>";
        private const string noOtherPlayersInTheRoomMessage = "There aren't enough players in the room.\nPlease find a new room.";

        //{0} Player name.
        private const string playerLeaveRoomMessage = "{0} leave the room.";
        private const int maxPlayer = 4;
        private const float swapPlayersUiDuration = 0.25f;

        [SerializeField]
        private GameObject playerPrefab = default;

        [SerializeField]
        private Transform[] playersPosition = default;

        [SerializeField]
        private Transform playerParentTransform = default;

        [SerializeField]
        private GameplayController gameplayController = default;

        [SerializeField]
        private CellPointerManager cellPointerManager = default;

        private Queue<Color> playersColor = new Queue<Color>(new Color[]
        {
            Color.red, Color.yellow, Color.blue, Color.green
        });

        private Dictionary<int, MineSweeperPlayer> players = new Dictionary<int, MineSweeperPlayer>();
        private MineSweeperPlayer localPlayer { get { return players[localPlayerId]; } }
        private List<MineSweeperPlayer> orderedPlayers = new List<MineSweeperPlayer>();
        private int currentPlayerAmount { get { return players.Count; } }
        private int localPlayerId { get { return LocalClientHandler.Instance.LocalClientPlayerId; } }
        private bool isSortingPlayer = false;

        public string GetPlayerNameWithColor(int playerId)
        {
            var player = players[playerId];
            return string.Format(playerNameWithColorFormat, ColorUtility.ToHtmlStringRGB(player.PlayerColor), player.PlayerName);
        }

        public bool IsPlayerIdDead(int playerId)
        {
            if (players.ContainsKey(playerId))
            {
                return players[playerId].IsDead;
            }

            return true;
        }

        public void AddScoreToPlayerById(int playerId)
        {
            var player = players[playerId];
            player.IncreaseScore();
            SortPlayersByScore();

            if (IsLastPlayerWin(player))
            {
                AllPlayersDead?.Invoke();
            }
        }

        public void IncreaseHiddenScoreToPlayerById(int playerId)
        {
            players[playerId].IncreaseHiddenScoreOnFlagBombCell();
        }

        public void DecreaseHiddenScoreFromPlayerById(int playerId)
        {
            players[playerId].DecreaseHiddenScoreOnUnflagBombCell();
        }

        public Color GetPlayerColorById(int playerId)
        {
            return players[playerId].PlayerColor;
        }

        public void EliminatePlayerById(int playerId)
        {
            var player = players[playerId];

            if (!player.IsDead)
            {
                player.SetPlayerState(PlayerState.Lose);

                if (playerId == localPlayer.OwnerId)
                {
                    LocalPlayerDie();
                }

                CheckAllPlayersLife();
            }
        }

        public void LocalPlayerDie()
        {
            IsLocalPlayerDead = true;
        }

        public MinesweeperSummary GetGameSummary()
        {
            return new MinesweeperSummary(localPlayer, orderedPlayers.ToArray(), orderedPlayers[0].CurrentScore);
        }

        private void Start()
        {
            SetupEvent();
            SendUpdateRoomToNetwork();
            SortPlayersByScore();
            ResetPlayer();
        }

        private void SendUpdateRoomToNetwork()
        {
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.UpdateRoom, LocalClientHandler.Instance.LocalClientPlayerId);
        }

        private void SortPlayersByScore()
        {
            orderedPlayers = orderedPlayers.OrderByDescending((player) => player.CurrentScore).ToList();

            if (!isSortingPlayer)
            {
                StartCoroutine(UpdatePlayersUiPosition());
            }
        }

        private void OnRoomUpdate(RoomUpdateMessage roomUpdateMessage)
        {
            HostId = roomUpdateMessage.MasterClientId;
            var playersInfo = roomUpdateMessage.playersInfo;
            var playersIdOnNetwork = playersInfo.Select(info => info.PlayerId);
            var playersIdOnClient = players.Select(player => player.Value.OwnerId);

            var disconnectPlayersId = playersIdOnClient.Except(playersIdOnNetwork).ToArray();

            for (int i = 0; i < disconnectPlayersId.Length; i++)
            {
                TerminateDisconnectPlayer(disconnectPlayersId[i]);
            }

            var newConnectPlayersId = playersIdOnNetwork.Except(playersIdOnClient).OrderBy(value => value);

            foreach (var playerId in newConnectPlayersId)
            {
                var playerName = playersInfo.Where(info => info.PlayerId == playerId).First();
                CreatePlayer(playerId, playerName.PlayerName);
            }

            if(IsOtherPlayersInTheRoom())
            {
                CheckAllPlayersLife();
            }
        }

        private bool IsOtherPlayersInTheRoom()
        {
            return currentPlayerAmount > 1;
        }

        private void ResetPlayer()
        {
            IsLocalPlayerDead = false;
        }

        private void CheckAllPlayersLife()
        {
            var isAllDead = players.All(player => player.Value.IsDead);

            if (isAllDead)
            {
                AllPlayersDead?.Invoke();
            }
        }

        /// <summary>
        /// If there is a single player still alive and has current score more than others just win instantly.
        /// </summary>
        /// <param name="lastPlayer">The last player.</param>
        /// <returns>Is lastest player is winner.</returns>
        private bool IsLastPlayerWin(MineSweeperPlayer lastPlayer)
        {
            if (GetPlayerDeadAmount() >= currentPlayerAmount - 1)
            {
                foreach (var otherPlayer in orderedPlayers)
                {
                    if (otherPlayer != lastPlayer)
                    {
                        if (otherPlayer.TotalScore >= lastPlayer.TotalScore)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private int GetPlayerDeadAmount()
        {
            return players.Where(player => player.Value.IsDead).Count();
        }

        private IEnumerator UpdatePlayersUiPosition()
        {
            isSortingPlayer = true;

            while (!IsAllPlayersUiInSortedPosition())
            {
                if (orderedPlayers.Count > 0)
                {
                    SortPlayersUiPosition();
                }

                yield return null;
            }

            isSortingPlayer = false;
        }

        private void SortPlayersUiPosition()
        {
            for (int i = 0; i < currentPlayerAmount; i++)
            {
                TransformUtilities.MoveTransformToDestination(orderedPlayers[i].transform, playersPosition[i], swapPlayersUiDuration);
            }
        }

        private void CreatePlayer(int playerId, string playerName)
        {
            var newObj = Instantiate(playerPrefab, playerParentTransform);
            newObj.transform.position = playersPosition[currentPlayerAmount].position;

            var newPlayer = newObj.GetComponent<MineSweeperPlayer>();
            players.Add(playerId, newPlayer);

            newPlayer.SetOwnerId(playerId);
            newPlayer.SetPlayerName(playerName);
            newPlayer.SetGameplayController(gameplayController);
            newPlayer.SetPlayersManager(this);
            newPlayer.SetCellPointerManager(cellPointerManager);
            newPlayer.SetPlayerColor(DefinePlayerColor());
            newPlayer.SetLocalPlayerTextActive(playerId == localPlayerId);

            orderedPlayers.Add(newPlayer);
        }

        private void TerminateDisconnectPlayer(int playerId)
        {
            var disconnectPlayer = players[playerId];
            var message = string.Format(playerLeaveRoomMessage, GetPlayerNameWithColor(playerId));
            AnnouncementTextManager.Instance.ShowAnnounceText(message, AnnouncementText.TinyTextWidth);

            Destroy(players[playerId].gameObject);
            orderedPlayers.Remove(players[playerId]);
            players.Remove(playerId);

            if (!IsOtherPlayersInTheRoom())
            {
                PopupManager.Instance.ShowPopup(noOtherPlayersInTheRoomMessage, () =>
                {
                    LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.LeaveRoom, LocalClientHandler.Instance.LocalClientPlayerId);
                    SceneManager.LoadScene(SceneList.LobbyScene);
                });
            }

            SortPlayersByScore();
        }

        private Color DefinePlayerColor()
        {
            if (playersColor.Count > 0)
            {
                return playersColor.Dequeue();
            }
            else
            {
                return Color.white;
            }
        }

        private bool IsAllPlayersUiInSortedPosition()
        {
            var orderedPlayersTransform = orderedPlayers.Select(player => player.transform).ToArray();
            return TransformUtilities.IsAllReachDestinations(orderedPlayersTransform, playersPosition);
        }

        private void OnGameOver()
        {
            foreach (var player in players.Values)
            {
                player.SummaryPlayerScore();

                if (!player.IsDead)
                {
                    player.SetPlayerState(PlayerState.Win);
                }
            }

            SortPlayersByScore();
        }

        private void SetupEvent()
        {
            gameplayController.StartGame += ResetPlayer;
            gameplayController.GameOver += OnGameOver;
            NetworkMessageHandler.Instance.OnRoomUpdateMessageReceived += OnRoomUpdate;
        }

        private void CleanUp()
        {
            if (gameplayController != null)
            {
                gameplayController.StartGame -= ResetPlayer;
                gameplayController.GameOver -= OnGameOver;
            }

            if (NetworkMessageHandler.Instance != null)
            {
                NetworkMessageHandler.Instance.OnRoomUpdateMessageReceived -= OnRoomUpdate;
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}