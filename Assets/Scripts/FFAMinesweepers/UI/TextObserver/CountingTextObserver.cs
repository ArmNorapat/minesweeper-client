using System;
using TMPro;
using TrueAxion.FFAMinesweepers.Event;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    /// <summary>
    /// Observe along int variable.
    /// </summary>
    public abstract class CountingTextObserver : MonoBehaviour
    {
        protected abstract EventAction<int> GetEvent();

        [SerializeField]
        private TextMeshProUGUI observerText = default;

        [SerializeField]
        private string stringFormat = generalNumberFormat;

        private const string generalNumberFormat = "000";

        private void UpdateText(int value)
        {
            observerText.text = value.ToString(stringFormat);
        }

        private void SetupEvent()
        {
            var targetEvent = GetEvent();
            targetEvent.SubscribeToEvent(UpdateText);
        }

        private void Awake()
        {
            SetupEvent();
        }
    }
}