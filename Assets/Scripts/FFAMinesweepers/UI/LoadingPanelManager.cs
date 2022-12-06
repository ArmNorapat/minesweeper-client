using System.Collections.Generic;
using TMPro;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.UI
{
    public class LoadingPanelManager : MonoSingleton<LoadingPanelManager>
    {
        [SerializeField]
        private GameObject panelObj = default;

        [SerializeField]
        private TextMeshProUGUI loadingStateText = default;

        public void ShowPanel(string loadingName)
        {
            panelObj.SetActive(true);
            loadingStateText.text = loadingName;
        }

        public void ClosePanel()
        {
            panelObj.SetActive(false);
        }

        private void Start()
        {
            ClosePanel();
        }
    }
}