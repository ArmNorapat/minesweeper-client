using UnityEngine;

namespace TrueAxion.FFAMinesweepers.CellScripts
{
    [CreateAssetMenu(fileName = "SpritesData", menuName = "CellSpritesData", order = 1)]
    public class CellSpriteData : ScriptableObject
    {
        public Sprite CloseSprite = default;
        public Sprite BombSprite = default;
        public Sprite TriggerBombSprite = default;
        public Sprite NotABombSprite = default;
        public Sprite FlaggedSprite = default;
        public Sprite EmptySprite = default;
        public Sprite[] NumberSprites = default;
    }
}