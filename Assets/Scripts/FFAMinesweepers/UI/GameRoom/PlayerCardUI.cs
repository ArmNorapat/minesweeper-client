using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.UI.GameRoom
{
    public class PlayerCardUI: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playerName = default;

        [SerializeField]
        private TMP_Text playerStatus = default;

        [SerializeField]
        private Color readyOutlineColor = default;

        [SerializeField]
        private Color notReadyOutlineColor = default;

        [SerializeField]
        private Outline outline = default;

        [SerializeField]
        private GameObject crown;

        [SerializeField]
        private GameObject identificationSymbol;

        public int PlayerID { get; set; }
        public bool IsMasterClient { get; private set; }

        public enum PlayerStatus
        {
            Ready,
            NotReady
        }

        public void Awake()
        {
            ResetCard();
        }

        public void ResetCard()
        {
            SetStatus(PlayerStatus.NotReady);
            crown.SetActive(false);
            identificationSymbol.SetActive(false);
        }

        public void SetStatus(PlayerStatus status)
        {
            switch (status)
            {
                case PlayerStatus.Ready:
                    playerStatus.text = PlayerStatus.Ready.ToString();
                    outline.effectColor = readyOutlineColor;
                    break;

                case PlayerStatus.NotReady:
                    playerStatus.text = "";
                    outline.effectColor = notReadyOutlineColor;
                    break;
            }
        }

        public void SetPlayerName(string name)
        {
            playerName.text = name;
        }

        public void ActiveIdentificationSymbol()
        {
            identificationSymbol.SetActive(true);
        }

        public void UpdateToMasterClient()
        {
            transform.SetAsFirstSibling();
            crown.SetActive(true);
            IsMasterClient = true;
            SetStatus(PlayerStatus.Ready);
        }
    }
}
