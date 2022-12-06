using TMPro;
using TrueAxion.FFAMinesweepers.Event;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI.TextObserver
{
    /// <summary>
    /// Observe along string variable.
    /// </summary>
    public abstract class StringTextObserver : MonoBehaviour
    {
        protected abstract EventAction<string> GetEvent();

        [SerializeField]
        private TextMeshProUGUI observerText = default;

        protected virtual void UpdateText(string value)
        {
            observerText.text = value;
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