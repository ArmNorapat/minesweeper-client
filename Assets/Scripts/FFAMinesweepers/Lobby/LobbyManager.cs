using TMPro;
using TrueAxion.FFAMinesweepers.Data;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.UI;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject lobbyPanel = default;

        [SerializeField]
        private GameObject setNamePanel = default;

        [SerializeField]
        private GameObject connectionPanel = default;

        [SerializeField]
        private TMP_InputField playerNameInputField = default;

        [SerializeField]
        private TMP_InputField ipAddressInputField = default;

        [SerializeField]
        private TMP_InputField portInputField = default;

        [SerializeField]
        private Button connectToServerButton = default;

        [SerializeField]
        private Button joinRoomButton = default;

        private const string connectToServerCompleteMessage = "Connect to server complete!";
        private const string connectToServerFailMessage = "Cannot connect to server!";
        private const string playerPrefsPlayerNameKey = "playerName";
        private const string ipAddressInvalidMessage = "Invalid IP address";
        private const string portInvalidMessage = "Invalid port";
        private const string loadingToRoomMessage = "Loading to room...";
        private const int minNameLength = 3;
        private const int maxNameLength = 9;
        private const int defaultGamePort = 13000;
        private static string nameInvalidMessage => $"Name invalid.\nName should has {minNameLength} - {maxNameLength} characters.";

        private string playerName;

        private void Start()
        {
            SetupEvent();
            LoadOldConnectionData();
            LoadingPanelManager.Instance.ClosePanel();

            if (!LocalClientHandler.Instance.IsClientExit)
            {
                OpenConnectionPanel();
            }
            else
            {
                OpenSetNamePanel();
            }
        }

        private void ConnectToServerButtonClicked()
        {
            CloseLobbyPanel();

            var ipAddress = ipAddressInputField.text;

            if (ipAddress.Length > 0)
            {
                if (int.TryParse(portInputField.text, out int port))
                {
                    if (LocalClientHandler.Instance.ConnectToServer(ipAddress, port))
                    {
                        AnnouncementTextManager.Instance.ShowAnnounceText(connectToServerCompleteMessage, AnnouncementText.MediumTextWidth);
                        OpenSetNamePanel();
                    }
                    else
                    {
                        PopupManager.Instance.ShowPopup(connectToServerFailMessage, OpenConnectionPanel);
                    }
                }
                else
                {
                    PopupManager.Instance.ShowPopup(portInvalidMessage);
                    OpenConnectionPanel();
                }
            }
            else
            {
                PopupManager.Instance.ShowPopup(ipAddressInvalidMessage);
                OpenConnectionPanel();
            }
        }

        private void JoinRoomButtonClicked()
        {
            UpdatePlayerNameFromInput();
            CloseLobbyPanel();

            if (!IsNameValid())
            {
                PopupManager.Instance.ShowPopup(nameInvalidMessage, OpenSetNamePanel);
            }
            else
            {
                LoadingPanelManager.Instance.ShowPanel(loadingToRoomMessage);
                SceneManager.LoadScene(SceneList.RoomScene);
            }
        }

        private void SetupEvent()
        {
            connectToServerButton.onClick.AddListener(ConnectToServerButtonClicked);
            joinRoomButton.onClick.AddListener(JoinRoomButtonClicked);
        }

        private void UpdatePlayerNameFromInput()
        {
            playerName = playerNameInputField.text;
            PlayerPrefs.SetString(playerPrefsPlayerNameKey, playerName);
        }

        private void LoadOldConnectionData()
        {
            ipAddressInputField.text = PlayerPrefs.GetString(LocalClientHandler.PlayerPrefServerIpKey, "");
            portInputField.text = PlayerPrefs.GetInt(LocalClientHandler.PlayerPrefsPortKey, defaultGamePort).ToString();
            playerNameInputField.text = PlayerPrefs.GetString(playerPrefsPlayerNameKey, playerName);
        }

        private bool IsNameValid()
        {
            return playerName.Length >= minNameLength && playerName.Length <= maxNameLength;
        }

        private void OpenConnectionPanel()
        {
            lobbyPanel.SetActive(true);
            connectionPanel.SetActive(true);
            setNamePanel.SetActive(false);
        }

        private void OpenSetNamePanel()
        {
            lobbyPanel.SetActive(true);
            connectionPanel.SetActive(false);
            setNamePanel.SetActive(true);
        }

        private void CloseLobbyPanel()
        {
            lobbyPanel.SetActive(false);
        }
    }
}