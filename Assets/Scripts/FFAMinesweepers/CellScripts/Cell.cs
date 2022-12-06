using TrueAxion.FFAMinesweepers.Gameplay;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Player;
using TrueAxion.FFAMinesweepers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TrueAxion.FFAMinesweepers.CellScripts
{
    public class Cell : MonoBehaviour
    {
        public const int EmptyCellValue = 0;
        public const int BombCellValue = -1;

        //A Cell that has been triggered with special mechanic while it on flagged state but it is not a bomb.
        public const int NotABombCellValue = -2;

        public enum CellState
        {
            CanClick,
            CannotClick,
            Flagged
        }

        public bool IsNumberCellValue => CellValue > EmptyCellValue;
        public int CellValue { get; private set; }
        public Vector2Int Position { get; private set; }

        [SerializeField]
        private Image image = default;

        [SerializeField]
        private CellSpriteData spriteData = default;

        [SerializeField]
        private Image cellBorder = default;

        //{0} is player name.
        private const string playerTriggerBombMessage = "{0} has been dead!";

        //{0} Rival player name, {1} Owner name
        private const string rivalEliminateOwnerMessage = "{0} found {1} didn't flag on bombcell!";

        //{0} Rival player name, {1} Owner name
        private const string rivalMisunderstandMessage = "{0} wrong, {1} flagged on bombcell!";
        private const int nonPlayerId = -1;

        private static readonly Color defaultBorderColor = Color.white;

        private CellManager cellManager;
        private int cellOwnerId = nonPlayerId;
        private CellState cellState = CellState.CanClick;
        private GameplayController gameplayController;
        private PlayersManager playersManager;

        public static Vector2Int GetCellPosById(int id, int rowAmount)
        {
            var x = id / rowAmount;
            var y = id % rowAmount;

            return new Vector2Int(x, y);
        }

        public static int GetCellIdByPos(Vector2Int pos, int rowAmount)
        {
            return (pos.x * rowAmount) + pos.y;
        }

        public void SetPosition(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }

        public void SetCellManager(CellManager manager)
        {
            cellManager = manager;
        }

        public void SetController(GameplayController controller)
        {
            gameplayController = controller;
        }

        public void SetPlayerManager(PlayersManager playersManager)
        {
            this.playersManager = playersManager;
        }

        public void InspectCell()
        {
            if (CanTriggerCell())
            {
                image.sprite = spriteData.EmptySprite;
            }
        }

        public void CancelInspectCell()
        {
            if (CanTriggerCell())
            {
                image.sprite = spriteData.CloseSprite;
            }
        }

        public bool IsFlagged()
        {
            return cellState == CellState.Flagged;
        }

        public bool IsBombCellAndAlreadyExposed()
        {
            if (cellState == CellState.CannotClick)
            {
                return CellValue == BombCellValue;
            }

            return false;
        }

        public bool CanTriggerCell()
        {
            return cellState == CellState.CanClick;
        }

        public void UpdateVisuals()
        {
            image.sprite = spriteData.CloseSprite;

            if (cellState == CellState.CannotClick)
            {
                switch (CellValue)
                {
                    case BombCellValue:
                        image.sprite = spriteData.TriggerBombSprite;
                        break;

                    case EmptyCellValue:
                        image.sprite = spriteData.EmptySprite;
                        break;

                    case NotABombCellValue:
                        image.sprite = spriteData.NotABombSprite;
                        break;

                    default:
                        int spriteIndex = CellValue - 1;
                        image.sprite = spriteData.NumberSprites[spriteIndex];
                        break;
                }
            }
            else if (cellState == CellState.Flagged)
            {
                image.sprite = spriteData.FlaggedSprite;
            }
        }

        public void VisualizeBombSprite()
        {
            if (CanTriggerCell())
            {
                image.sprite = spriteData.BombSprite;
            }
        }

        public void SetCellValue(int value)
        {
            CellValue = value;
        }

        /// <summary>
        /// Call when cell is clicked by mouse button.
        /// </summary>
        /// <param name="networkAction">Network action.</param>
        public void TakeAction(NetworkAction networkAction)
        {
            if (CanSendActionToNetwork(networkAction))
            {
                cellManager.SendActionToNetwork(Position, networkAction);
            }
        }

        /// <summary>
        /// Every time player trigger on empty or number tile, player get 1 point.
        /// </summary>
        /// <param name="isTriggeredByClick">Define cell triggered by player's click or not.</param>
        /// <param name="playerId">Who trigger this cell.</param>
        public void TriggerCell(bool isTriggeredByClick, int playerId)
        {
            if (CanTriggerCell())
            {
                cellState = CellState.CannotClick;
                UpdateVisuals();

                if (CellValue == BombCellValue)
                {
                    var playerWhoTrigger = playersManager.GetPlayerNameWithColor(playerId);
                    var message = string.Format(playerTriggerBombMessage, playerWhoTrigger);
                    AnnouncementTextManager.Instance.ShowAnnounceText(message, AnnouncementText.TinyTextWidth);

                    playersManager.EliminatePlayerById(playerId);
                    SetCellBorderColor(playersManager.GetPlayerColorById(playerId));
                    gameplayController.DecreaseFlagAmount();
                }
                else
                {
                    if (CellValue == EmptyCellValue)
                    {
                        cellManager.OpenEmptyCellArea(Position, playerId);

                        if (isTriggeredByClick)
                        {
                            AddScoreAndSetCellColorByPlayerId(playerId);
                        }
                    }
                    else
                    {
                        AddScoreAndSetCellColorByPlayerId(playerId);
                    }

                    gameplayController.IncreaseTriggeredCellAmount();
                }
            }
        }

        /// <summary>
        /// Player will get 5 points in every time player flag a mine correctly.
        /// Player can right click on other player's flag.
        /// If that flag is correct, Clicked player lose. If not, flag's owner lose.
        /// </summary>
        public void FlagCell(int playerId)
        {
            if (playersManager.IsPlayerIdDead(playerId))
            {
                return;
            }

            if (CanTriggerCell())
            {
                gameplayController.DecreaseFlagAmount();
                cellState = CellState.Flagged;
                cellOwnerId = playerId;
                SetCellBorderColor(playersManager.GetPlayerColorById(playerId));

                if (CellValue == BombCellValue)
                {
                    playersManager.IncreaseHiddenScoreToPlayerById(playerId);
                }
                else
                {
                    playersManager.DecreaseHiddenScoreFromPlayerById(playerId);
                }
            }
            else if (cellState == CellState.Flagged)
            {
                if (playerId == cellOwnerId)
                {
                    gameplayController.IncreaseFlagAmount();
                    cellState = CellState.CanClick;
                    cellOwnerId = nonPlayerId;
                    CloseBorder();

                    if (CellValue == BombCellValue)
                    {
                        playersManager.DecreaseHiddenScoreFromPlayerById(playerId);
                    }
                    else
                    {
                        playersManager.IncreaseHiddenScoreToPlayerById(playerId);
                    }
                }
                else
                {
                    var rivalPlayerName = playersManager.GetPlayerNameWithColor(playerId);
                    var ownerPlayerName = playersManager.GetPlayerNameWithColor(cellOwnerId);

                    if (CellValue == BombCellValue)
                    {
                        playersManager.EliminatePlayerById(playerId);

                        var message = string.Format(rivalMisunderstandMessage, rivalPlayerName, ownerPlayerName);
                        AnnouncementTextManager.Instance.ShowAnnounceText(message, AnnouncementText.LargeTextWidth);
                    }
                    else
                    {
                        EliminateCellOwnerAndDisableCell();

                        var message = string.Format(rivalEliminateOwnerMessage, rivalPlayerName, ownerPlayerName);
                        AnnouncementTextManager.Instance.ShowAnnounceText(message, AnnouncementText.LargeTextWidth);
                    }
                }
            }

            UpdateVisuals();
        }

        public void TriggerSurroundCell(int playerId)
        {
            cellManager.IterateAvailableSurroundCells(Position, (cell) =>
            {
                cell.TriggerCellFromInspected(playerId);
            });
        }

        public void ResetCell()
        {
            CellValue = EmptyCellValue;
            cellState = CellState.CanClick;
            cellOwnerId = nonPlayerId;
            UpdateVisuals();
            CloseBorder();
        }

        private void SetCellBorderColor(Color color)
        {
            cellBorder.color = color;
            cellBorder.enabled = true;
        }

        private void CloseBorder()
        {
            cellBorder.enabled = false;
        }

        private void AddScoreAndSetCellColorByPlayerId(int playerId)
        {
            playersManager.AddScoreToPlayerById(playerId);
            SetCellBorderColor(playersManager.GetPlayerColorById(playerId));
        }

        private void TriggerCellFromInspected(int playerId)
        {
            switch (cellState)
            {
                case CellState.Flagged:
                    if (CellValue != BombCellValue)
                    {
                        EliminateCellOwnerAndDisableCell();
                    }
                    break;

                case CellState.CanClick:
                    TriggerCell(true, playerId);
                    break;
            }
        }

        private void EliminateCellOwnerAndDisableCell()
        {
            playersManager.EliminatePlayerById(cellOwnerId);
            CellValue = NotABombCellValue;
            cellState = CellState.CannotClick;
            gameplayController.IncreaseFlagAmount();
            gameplayController.IncreaseTriggeredCellAmount();
            UpdateVisuals();
        }

        private bool CanSendActionToNetwork(NetworkAction networkAction)
        {
            switch (cellState)
            {
                case CellState.Flagged:
                    return networkAction == NetworkAction.FlagCell;

                case CellState.CannotClick:
                    return networkAction == NetworkAction.TriggerSurroundCell && CanTriggerSurroundCell();

                case CellState.CanClick:
                    return networkAction != NetworkAction.TriggerSurroundCell;

                default:
                    return false;
            }
        }

        private bool CanTriggerSurroundCell()
        {
            var flagCount = 0;

            cellManager.IterateAvailableSurroundCells(Position, (cell) =>
            {
                flagCount += cell.IsFlagged() || cell.IsBombCellAndAlreadyExposed() ? 1 : 0;
            });

            return flagCount == CellValue && IsNumberCellValue;
        }
    }
}