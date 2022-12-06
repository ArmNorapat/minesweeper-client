using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.UI
{
    public class PopupManager : MonoSingleton<PopupManager>
    {
        public bool IsPopupActive { get { return popupPanel.activeInHierarchy; } }

        [SerializeField]
        private GameObject popupPanel = default;

        [SerializeField]
        private GameObject choicePanel = default;

        [SerializeField]
        private TextMeshProUGUI messageText = default;

        [SerializeField]
        private TextMeshProUGUI buttonText = default;

        [SerializeField]
        private Button popupButton = default;

        [SerializeField]
        private Button popupYesButton = default;

        [SerializeField]
        private Button popupNoButton = default;

        private event Action popupButtonClicked;
        private event Action popupYesButtonClicked;

        private const string defaultPopupButtonName = "OK";

        public void ShowPopup(string message)
        {
            ShowNormalPopup();

            messageText.text = message;
            buttonText.text = defaultPopupButtonName;

            popupButtonClicked = null;
        }

        public void ShowPopup(string message, Action callback)
        {
            ShowNormalPopup();

            messageText.text = message;
            buttonText.text = defaultPopupButtonName;

            popupButtonClicked = callback;
        }

        public void ShowPopup(string message, string buttonName, Action callback)
        {
            ShowNormalPopup();

            messageText.text = message;
            buttonText.text = buttonName;

            popupButtonClicked = callback;
        }

        public void ShowNonButtonPopup(string message)
        {
            popupPanel.SetActive(true);
            popupButton.gameObject.SetActive(false);
            choicePanel.SetActive(false);

            messageText.text = message;
        }

        public void ShowChoicePopup(string message, Action yesButtonCallback)
        {
            popupPanel.SetActive(true);
            popupButton.gameObject.SetActive(false);
            choicePanel.SetActive(true);

            messageText.text = message;

            popupYesButtonClicked = yesButtonCallback;
        }

        public void ClosePopup()
        {
            popupButton.gameObject.SetActive(false);
            popupPanel.SetActive(false);
        }

        private void ShowNormalPopup()
        {
            popupButton.gameObject.SetActive(true);
            popupPanel.SetActive(true);
            choicePanel.SetActive(false);
        }

        private void Start()
        {
            ClosePopup();

            popupButton.onClick.AddListener(() =>
            {
                popupButtonClicked?.Invoke();
                ClosePopup();
            });

            popupYesButton.onClick.AddListener(() =>
            {
                popupYesButtonClicked?.Invoke();
                ClosePopup();
            });

            popupNoButton.onClick.AddListener(ClosePopup);
        }
    }
}