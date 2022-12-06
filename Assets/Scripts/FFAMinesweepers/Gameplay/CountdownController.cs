using System;
using System.Collections;
using System.Timers;
using TrueAxion.FFAMinesweepers.CellScripts;
using TrueAxion.FFAMinesweepers.Event;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.UI;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Gameplay
{
    public class CountdownController : MonoBehaviour
    {
        public readonly EventAction<int> CountdownNumberChanged = new EventAction<int>();
        public event Action CountdownFinished;
        public event Action CountdownStarted;

        [SerializeField]
        private Animator countdownAnimator = default;

        [SerializeField]
        private GameObject countdownPanel = default;

        [SerializeField]
        private CellManager cellManager = default;

        private const float countdownDuration = 3;
        private const string countdownAnimParameter = "Countdown";
        private double currentLocalDateTimeUnix { get { return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; } }

        private void Start()
        {
            SetupEvent();

            countdownPanel.SetActive(false);
        }

        private void SetupEvent()
        {
            NetworkMessageHandler.Instance.OnGetCountdownTimeMessageReceived += OnGetCountdownTime;
            cellManager.ResetBoardSuccess += SendReadyToCountDownToNetwork;
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (NetworkMessageHandler.Instance != null)
            {
                NetworkMessageHandler.Instance.OnGetCountdownTimeMessageReceived -= OnGetCountdownTime;
            }

            if (cellManager != null)
            {
                cellManager.ResetBoardSuccess -= SendReadyToCountDownToNetwork;
            }
        }

        private void SendReadyToCountDownToNetwork()
        {
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.ReadyToCountDown, LocalClientHandler.Instance.LocalClientPlayerId);
        }

        private void OnGetCountdownTime(GetCountdownTimeMessage getCountdownTimeMessage)
        {
            StartCoroutine(Countdown(countdownDuration));
        }

        private IEnumerator Countdown(float countdownDuration)
        {
            PopupManager.Instance.ClosePopup();
            LoadingPanelManager.Instance.ClosePanel();

            OpenCountdownPanel();

            var currentCountdownDuration = countdownDuration;
            var nextCountdownUpdateNumber = Mathf.Floor(currentCountdownDuration);

            SetCountdownTextValue(currentCountdownDuration);

            while (currentCountdownDuration > 0)
            {
                if (currentCountdownDuration < nextCountdownUpdateNumber)
                {
                    nextCountdownUpdateNumber = currentCountdownDuration - 1;
                    SetCountdownTextValue(currentCountdownDuration);
                }

                currentCountdownDuration -= Time.deltaTime;

                yield return null;
            }

            CloseCountdownPanel();
        }

        private void SetCountdownTextValue(float value)
        {
            CountdownNumberChanged.FireEvent(Mathf.FloorToInt(value + 1));
            countdownAnimator.SetTrigger(countdownAnimParameter);
        }

        private void OpenCountdownPanel()
        {
            countdownPanel.SetActive(true);
            CountdownStarted?.Invoke();
        }

        private void CloseCountdownPanel()
        {
            countdownPanel.SetActive(false);
            CountdownFinished?.Invoke();
        }
    }
}