using System;
using TMPro;
using TrueAxion.FFAMinesweepers.Data;
using TrueAxion.FFAMinesweepers.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.UI.GameRoom
{
    public class GameRoomUI: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text roomID = default;

        [SerializeField]
        private GameObject playerList = default;

        [SerializeField]
        private Button startButton = default;

        [SerializeField]
        private GameObject startButtonGO = default;

        [SerializeField]
        private GameObject readyButtonGO = default;

        [SerializeField]
        private GameObject cancelButtonGO = default;

        private int lastCardIndex => playerList.transform.childCount - 1;
        private int currentPlayerAmount = 0;

        private const string roomIDFormat = "ROOM: {0}";

        public void ResetRoomUI()
        {
            readyButtonGO.SetActive(true);
            startButtonGO.SetActive(false);
            ResetAllPlayerCards();
        }

        public void SetRoomID(int id)
        {
            roomID.text = string.Format(roomIDFormat, id);
        }

        public void AddNewPlayer(PlayerInfo playerInfo)
        {
            if (currentPlayerAmount > lastCardIndex)
            {
                throw new Exception("Shouldn't add new player to the full room.");
            }

            var currentPlayerCard = playerList.transform.GetChild(lastCardIndex).gameObject;
            var playerCardUI = currentPlayerCard.GetComponent<PlayerCardUI>();

            playerCardUI.SetPlayerName(playerInfo.PlayerName);
            playerCardUI.PlayerID = playerInfo.PlayerId;

            currentPlayerCard.transform.SetSiblingIndex(currentPlayerAmount);
            currentPlayerCard.SetActive(true);

            if (playerCardUI.PlayerID == LocalClientHandler.Instance.LocalClientPlayerId)
            {
                playerCardUI.ActiveIdentificationSymbol();
            }

            currentPlayerAmount++;
        }

        public void RemovePlayer(PlayerInfo playerInfo)
        {
            int id = playerInfo.PlayerId;

            IterateCardsInPlayerList(id, (playerCardUI, card) =>
            {
                playerCardUI.ResetCard();
                card.gameObject.SetActive(false);
                card.SetAsLastSibling();
                currentPlayerAmount--;
            });
        }

        public void SetMasterClient(int playerID)
        {
            IterateCardsInPlayerList(playerID, (playerCardUI, card) =>
            {
                if (!playerCardUI.IsMasterClient)
                {
                    playerCardUI.UpdateToMasterClient();
                }

                if (playerID == LocalClientHandler.Instance.LocalClientPlayerId)
                {
                    readyButtonGO.SetActive(false);
                    cancelButtonGO.SetActive(false);
                    startButtonGO.SetActive(true);
                }
            });
        }

        public void SetButtonInteractable(bool isInteractable)
        {
            if (startButtonGO.activeSelf)
            {
                startButton.interactable = isInteractable;
            }
        }

        public void SetReadyButtonActive(bool isActive)
        {
            readyButtonGO.SetActive(isActive);
            cancelButtonGO.SetActive(!isActive);
        }

        public void IterateCardsInPlayerList(int playerID, Action<PlayerCardUI, Transform> action)
        {
            foreach (Transform card in playerList.transform)
            {
                PlayerCardUI playerCardUI = card.GetComponent<PlayerCardUI>();

                if (playerCardUI.PlayerID == playerID)
                {
                    action(playerCardUI, card);
                    break;
                }
            }
        }

        private void ResetAllPlayerCards()
        {
            foreach (Transform card in playerList.transform)
            {
                PlayerCardUI playerCardUI = card.GetComponent<PlayerCardUI>();
                playerCardUI.ResetCard();
                card.gameObject.SetActive(false);
            }
        }
    }
}
