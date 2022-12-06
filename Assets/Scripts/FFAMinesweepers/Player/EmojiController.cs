using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.Player
{
    public class EmojiController : MonoBehaviour
    {
        [SerializeField]
        private Image emojiImage = default;

        [SerializeField]
        private EmojiSpriteData emojiSpriteData = default;

        [SerializeField]
        private MineSweeperPlayer mineSweeperPlayer = default;

        private void Start()
        {
            SubscribeEvent();
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void SubscribeEvent()
        {
            mineSweeperPlayer.PlayerStateChanged += RefreshSprite;
        }

        private void CleanUp()
        {
            if (mineSweeperPlayer != null)
            {
                mineSweeperPlayer.PlayerStateChanged -= RefreshSprite;
            }
        }

        private void RefreshSprite(PlayerState playerState)
        {
            switch (playerState)
            {
                case PlayerState.Lose:
                    SetSprite(emojiSpriteData.DeadSprite);
                    break;

                case PlayerState.Win:
                    SetSprite(emojiSpriteData.WinSprite);
                    break;

                case PlayerState.Inspecting:
                    SetSprite(emojiSpriteData.WowSprite);
                    break;

                default:
                    SetSprite(emojiSpriteData.SmileSprite);
                    break;
            }
        }

        private void SetSprite(Sprite sprite)
        {
            emojiImage.sprite = sprite;
        }
    }
}