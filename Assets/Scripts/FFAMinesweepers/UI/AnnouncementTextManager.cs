using System.Collections;
using System.Collections.Generic;
using TMPro;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.UI
{
    public class AnnouncementTextManager : MonoSingleton<AnnouncementTextManager>
    {
        [SerializeField]
        private AnnouncementText announcementTextPrefab = default;

        private const float textLifeTime = 5;

        public void ShowAnnounceText(string message, float textWidth)
        {
            var newAnnounceText = Instantiate(announcementTextPrefab, transform);

            if (newAnnounceText != null)
            {
                newAnnounceText.SetTextScale(textWidth);
                newAnnounceText.SetMessage(message);
            }

            Destroy(newAnnounceText.gameObject, textLifeTime);
        }

        public void ShowAnnounceText(string message, Color backgroundColor, float textWidth)
        {
            var newAnnounceText = Instantiate(announcementTextPrefab, transform);

            if (newAnnounceText != null)
            {
                newAnnounceText.SetTextScale(textWidth);
                newAnnounceText.SetMessage(message);
                newAnnounceText.SetBackgroundColor(backgroundColor);
            }

            Destroy(newAnnounceText.gameObject, textLifeTime);
        }
    }
}