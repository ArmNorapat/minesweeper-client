using System;
using System.Collections.Generic;
using System.Linq;
using TrueAxion.FFAMinesweepers.Data;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.UI;
using TrueAxion.FFAMinesweepers.UI.GameRoom;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrueAxion.FFAMinesweepers.GameRoom
{
    public class GameRoomManager : MonoBehaviour
    {
        [SerializeField]
        private GameRoomUI gameRoomUI = default;

        private HashSet<PlayerInfo> playersInRoom = new HashSet<PlayerInfo>();

        //{0} Player name
        private const string otherPlayerLeaveTheRoomMessage = "{0} leave the room!";

        //{0} Player name
        private const string otherPlayerJoinTheRoomMessage = "{0} join the room!";
        private const string allPlayersReadyMessage = "All players are ready!";
        private const string playerPrefsPlayerNameKey = "playerName";

        public void StartGameRequest()
        {
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.StartGameRequest, LocalClientHandler.Instance.LocalClientPlayerId);
        }

        public void ReadyToPlay()
        {
            gameRoomUI.SetReadyButtonActive(false);
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.ReadyToPlay, LocalClientHandler.Instance.LocalClientPlayerId);
        }

        public void CancelReadyToPlay()
        {
            gameRoomUI.SetReadyButtonActive(true);
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.CancelReadyToPlay, LocalClientHandler.Instance.LocalClientPlayerId);
        }

        public void LeaveRoom()
        {
            gameRoomUI.ResetRoomUI();
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.LeaveRoom, LocalClientHandler.Instance.LocalClientPlayerId);
            SceneManager.LoadScene(SceneList.LobbyScene);
        }

        private void Start()
        {
            SetupEvent();
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.JoinRoom, LocalClientHandler.Instance.LocalClientPlayerId, PlayerPrefs.GetString(playerPrefsPlayerNameKey));
        }

        private void OnJoinRoom(JoinRoomMessage joinRoomMessage)
        {
            gameRoomUI.SetRoomID(joinRoomMessage.roomID);
            LoadingPanelManager.Instance.ClosePanel();
        }

        private void OnRoomUpdate(RoomUpdateMessage roomUpdateMessage)
        {
            if (LocalClientHandler.Instance.LocalClientPlayerId != roomUpdateMessage.MasterClientId)
            {
                CancelReadyToPlay();
            }

            var newPlayersInfo = roomUpdateMessage.playersInfo;

            var disconnectPlayersInfo = playersInRoom.Except(newPlayersInfo);

            foreach (PlayerInfo disconnectPlayer in disconnectPlayersInfo)
            {
                gameRoomUI.RemovePlayer(disconnectPlayer);

                var message = string.Format(otherPlayerLeaveTheRoomMessage, disconnectPlayer.PlayerName);
                AnnouncementTextManager.Instance.ShowAnnounceText(message, AnnouncementText.MediumTextWidth);
            }

            var newConnectPlayersInfo = newPlayersInfo.Except(playersInRoom);

            foreach (PlayerInfo connectPlayer in newConnectPlayersInfo)
            {
                gameRoomUI.AddNewPlayer(connectPlayer);

                if (connectPlayer.PlayerId != LocalClientHandler.Instance.LocalClientPlayerId)
                {
                    var message = string.Format(otherPlayerJoinTheRoomMessage, connectPlayer.PlayerName);
                    AnnouncementTextManager.Instance.ShowAnnounceText(message, AnnouncementText.MediumTextWidth);
                }
            }

            playersInRoom = new HashSet<PlayerInfo>(newPlayersInfo);
            gameRoomUI.SetMasterClient(roomUpdateMessage.MasterClientId);
        }

        private void OnReadyToStart(ReadyToStartMessage readyToStartMessage)
        {
            gameRoomUI.SetButtonInteractable(readyToStartMessage.IsReadyToStart);

            if (readyToStartMessage.IsReadyToStart)
            {
                AnnouncementTextManager.Instance.ShowAnnounceText(allPlayersReadyMessage, AnnouncementText.MediumTextWidth);
            }
        }

        private void OnPlayerReadyToPlay(ReadyToPlayMessage readyToPlayMessage)
        {
            int readyPlayerID = readyToPlayMessage.playerID;

            gameRoomUI.IterateCardsInPlayerList(readyPlayerID, (playerCardUI, card) =>
            {
                playerCardUI.SetStatus(PlayerCardUI.PlayerStatus.Ready);
            });
        }

        private void OnPlayerCancelReadyToPlay(CancelReadyToPlayMessage cancelReadyToPlayMessage)
        {
            int notReadyPlayerID = cancelReadyToPlayMessage.playerID;

            gameRoomUI.IterateCardsInPlayerList(notReadyPlayerID, (playerCardUI, card) =>
            {
                playerCardUI.SetStatus(PlayerCardUI.PlayerStatus.NotReady);
            });
        }

        private void OnStartGame(StartGameMessage startGameMessage)
        {
            if (startGameMessage.IsGameStart)
            {
                SceneManager.LoadScene(SceneList.GamePlayScene);
            }
        }

        private void SetupEvent()
        {
            NetworkMessageHandler.Instance.OnJoinRoomMessageReceived += OnJoinRoom;
            NetworkMessageHandler.Instance.OnRoomUpdateMessageReceived += OnRoomUpdate;
            NetworkMessageHandler.Instance.OnReadyToStartMessageReceived += OnReadyToStart;
            NetworkMessageHandler.Instance.OnReadyToPlayMessageReceived += OnPlayerReadyToPlay;
            NetworkMessageHandler.Instance.OnCancelReadyToPlayMessageReceived += OnPlayerCancelReadyToPlay;
            NetworkMessageHandler.Instance.OnStartGameMessageReceived += OnStartGame;
        }

        private void OnDestroy()
        {
            if (NetworkMessageHandler.Instance != null)
            {
                NetworkMessageHandler.Instance.OnJoinRoomMessageReceived -= OnJoinRoom;
                NetworkMessageHandler.Instance.OnRoomUpdateMessageReceived -= OnRoomUpdate;
                NetworkMessageHandler.Instance.OnReadyToStartMessageReceived -= OnReadyToStart;
                NetworkMessageHandler.Instance.OnReadyToPlayMessageReceived -= OnPlayerReadyToPlay;
                NetworkMessageHandler.Instance.OnCancelReadyToPlayMessageReceived -= OnPlayerCancelReadyToPlay;
                NetworkMessageHandler.Instance.OnStartGameMessageReceived -= OnStartGame;
            }
        }
    }
}