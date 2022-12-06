using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.UI
{
    public class AnnouncementText : MonoBehaviour
    {
        public const float LargeTextWidth = 500;
        public const float MediumTextWidth = 400;
        public const float TinyTextWidth = 300;

        [SerializeField]
        private RectTransform textTransform = default;

        [SerializeField]
        private TextMeshProUGUI mainText = default;

        [SerializeField]
        private Image backGround = default;

        private const float defaultTextHeight = 50;

        public void SetTextScale(float width)
        {
            textTransform.sizeDelta = new Vector2(width, defaultTextHeight);
        }

        public void SetMessage(string message)
        {
            mainText.text = message;
        }

        public void SetBackgroundColor(Color color)
        {
            backGround.color = color;
        }
    }
}