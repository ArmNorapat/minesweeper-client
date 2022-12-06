using System.Collections;
using System.Collections.Generic;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.UI
{
    public class LeaveRoomButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private Button leaveRoomButton = default;

        [SerializeField]
        private string destinationSceneName = SceneList.LobbyScene;

        [SerializeField]
        private GameObject confirmationPanel = default;

        [SerializeField]
        private Button yesButton = default;

        [SerializeField]
        private Button noButton = default;

        private void Start()
        {
            confirmationPanel.SetActive(false);

            SetupEvent();
        }

        private void SetupEvent()
        {
            leaveRoomButton.onClick.AddListener(OpenConfirmationPanel);
            yesButton.onClick.AddListener(LeaveRoom);
            noButton.onClick.AddListener(CloseConfirmationPanel);
        }

        private void LeaveRoom()
        {
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.LeaveRoom, LocalClientHandler.Instance.LocalClientPlayerId);
            SceneManager.LoadScene(destinationSceneName);
            CloseConfirmationPanel();
        }

        private void CloseConfirmationPanel()
        {
            confirmationPanel.SetActive(false);
        }

        private void OpenConfirmationPanel()
        {
            confirmationPanel.SetActive(true);
        }
    }
}