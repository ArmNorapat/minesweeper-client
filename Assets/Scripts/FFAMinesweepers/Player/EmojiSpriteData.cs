using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Player
{
    [CreateAssetMenu(fileName = "EmojiSpriteData", menuName = "EmojiSpriteData", order = 1)]
    public class EmojiSpriteData : ScriptableObject
    {
        public Sprite SmileSprite = default;
        public Sprite DeadSprite = default;
        public Sprite WowSprite = default;
        public Sprite WinSprite = default;
    }
}
