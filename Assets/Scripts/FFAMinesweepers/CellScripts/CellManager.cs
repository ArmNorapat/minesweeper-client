using System;
using System.Collections.Generic;
using System.Linq;
using TrueAxion.FFAMinesweepers.Gameplay;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Networking.NetworkMessage;
using TrueAxion.FFAMinesweepers.Player;
using TrueAxion.FFAMinesweepers.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrueAxion.FFAMinesweepers.CellScripts
{
    public class CellManager : MonoBehaviour
    {
        public event Action ResetBoardSuccess;

        public const int BombAmount = 100;

        private const int boardWidth = 32;
        private const int boardHeight = 16;
        private const int score = 1;
        private const int minSeedNumber = 1000;
        private const int maxSeedNumber = 9999;
        private const int playerTestingId = 0;

        private readonly Vector2Int[] surroundPos = {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1)
        };

        [SerializeField]
        private Transform cellParent = default;

        [SerializeField]
        private GameObject cellPrefab = default;

        [SerializeField]
        private GameplayController gameplayController = default;

        [SerializeField]
        private PlayersManager playersManager = default;

        private HashSet<Cell> checkedEmptyCell = new HashSet<Cell>();

        private Cell[, ] cells;

        /// <summary>
        /// Send message with x, y mouse button to server and other client.
        /// </summary>
        /// <param name="position">Position of cell.</param>
        /// <param name="action">Player action.</param>
        public void SendActionToNetwork(Vector2Int position, NetworkAction action)
        {
            var cellId = Cell.GetCellIdByPos(position, boardHeight);

            switch (action)
            {
                case NetworkAction.TriggerSurroundCell:
                    var surroundCellId = string.Join($"{NetworkMessageHandler.ArraySeperator}", GetTriggerableCellAroundPosition(position));
                    LocalClientHandler.Instance.SendMessageToNetwork(action, LocalClientHandler.Instance.LocalClientPlayerId, cellId, surroundCellId);
                    break;

                default:
                    LocalClientHandler.Instance.SendMessageToNetwork(action, LocalClientHandler.Instance.LocalClientPlayerId, cellId);
                    break;
            }
        }

        /// <summary>
        /// Receive clicked cell from server.
        /// </summary>
        /// <param name="x">X position of cell.</param>
        /// <param name="y">Y position of cell.</param>
        /// <param name="networkAction">User network action.</param>
        /// <param name="playerId">ID of player who did action.</param>
        public void ReceivePlayerAction(NetworkAction networkAction, int playerId, int cellId)
        {
            var cellPosition = Cell.GetCellPosById(cellId, boardHeight);

            switch (networkAction)
            {
                //Leftclick
                case NetworkAction.TriggerCell:
                    cells[cellPosition.x, cellPosition.y].TriggerCell(true, playerId);
                    break;

                    //Rightclick
                case NetworkAction.FlagCell:
                    cells[cellPosition.x, cellPosition.y].FlagCell(playerId);
                    break;

                    //Right and Left click
                case NetworkAction.TriggerSurroundCell:
                    cells[cellPosition.x, cellPosition.y].TriggerSurroundCell(playerId);
                    break;
            }
        }

        public void OpenAllBombs()
        {
            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    Cell cell = cells[x, y];

                    if (cell.CellValue == Cell.BombCellValue)
                    {
                        cell.VisualizeBombSprite();
                    }
                }
            }
        }

        public int NonBombCellCount()
        {
            return cells.Length - BombAmount;
        }

        public void OpenEmptyCellArea(Vector2Int postion, int playerId)
        {
            Cell currentCell = cells[postion.x, postion.y];
            checkedEmptyCell.Add(currentCell);

            IterateAvailableSurroundCells(currentCell.Position, (cell) =>
            {
                if (!checkedEmptyCell.Contains(cell))
                {
                    if (cell.CellValue != Cell.BombCellValue)
                    {
                        cell.TriggerCell(false, playerId);
                        checkedEmptyCell.Add(cell);
                    }
                    if (cell.CellValue == Cell.EmptyCellValue)
                    {
                        OpenEmptyCellArea(cell.Position, playerId);
                    }
                }
            });
        }

        /// <summary>
        /// Get surround cell of a position and do action to those cell.
        /// </summary>
        /// <param name="middlePosition">The middle position.</param>
        /// <param name="action">Action we do to surround cell.</param>
        public void IterateAvailableSurroundCells(Vector2Int middlePosition, System.Action<Cell> action)
        {
            foreach (Vector2Int pos in surroundPos)
            {
                var xPos = middlePosition.x + pos.x;
                var yPos = middlePosition.y + pos.y;

                if (!IsInBound(xPos, yPos))
                {
                    continue;
                }

                var currentCell = cells[xPos, yPos];

                action?.Invoke(currentCell);
            }
        }

        private int[] GetTriggerableCellAroundPosition(Vector2Int middlePosition)
        {
            List<int> triggerableCell = new List<int>();

            IterateAvailableSurroundCells(middlePosition, (cell) =>
            {
                if (cell.CanTriggerCell())
                {
                    triggerableCell.Add(Cell.GetCellIdByPos(cell.Position, boardHeight));
                }
            });

            return triggerableCell.ToArray();
        }

        public void RequestSeedFromNetwork()
        {
            LocalClientHandler.Instance.SendMessageToNetwork(NetworkAction.GetSeedNumber, LocalClientHandler.Instance.LocalClientPlayerId);
        }

        private void Start()
        {
            SetupEvent();
            GenerateBoard();
            RequestSeedFromNetwork();
        }

        private void ResetBoard(int seedNumber)
        {
            SetSeed(seedNumber);

            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    cells[x, y].ResetCell();
                }
            }

            GenerateBombs();
            checkedEmptyCell.Clear();
            ResetBoardSuccess?.Invoke();
        }

        private void SetupEvent()
        {
            gameplayController.GameOver += OpenAllBombs;
            NetworkMessageHandler.Instance.OnFlagCellMessageReceived += OnFlagCell;
            NetworkMessageHandler.Instance.OnTriggerCellMessageReceived += OnTriggerCell;
            NetworkMessageHandler.Instance.OnTriggerSurroundCellMessageReceived += OnTriggerSurroundCell;
            NetworkMessageHandler.Instance.OnGetSeedNumberMessageReceived += OnGetSeedNumber;
        }

        private void CleanUp()
        {
            if (gameplayController != null)
            {
                gameplayController.GameOver -= OpenAllBombs;
            }

            if (NetworkMessageHandler.Instance != null)
            {
                NetworkMessageHandler.Instance.OnFlagCellMessageReceived -= OnFlagCell;
                NetworkMessageHandler.Instance.OnTriggerCellMessageReceived -= OnTriggerCell;
                NetworkMessageHandler.Instance.OnTriggerSurroundCellMessageReceived -= OnTriggerSurroundCell;
                NetworkMessageHandler.Instance.OnGetSeedNumberMessageReceived -= OnGetSeedNumber;
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void OpenAllCells(int playerId)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    if (cells[x, y].CellValue != Cell.BombCellValue)
                    {
                        cells[x, y].TriggerCell(false, playerId);
                    }
                }
            }
        }

        private void SetSeed(int seedNumber)
        {
            Random.InitState(seedNumber);
        }

        private void GenerateBoard()
        {
            cells = new Cell[boardWidth, boardHeight];

            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    var newCellObject = Instantiate(cellPrefab, cellParent);
                    var newCell = newCellObject.GetComponent<Cell>();
                    newCell.SetPosition(x, y);
                    newCell.SetCellManager(this);
                    newCell.SetController(gameplayController);
                    newCell.SetPlayerManager(playersManager);
                    cells[x, y] = newCell;
                }
            }
        }

        private void GenerateBombs()
        {
            IEnumerable<int> randomCellsID = Enumerable.Range(0, cells.Length)
                                                       .OrderBy((i) => Random.value)
                                                       .Take(BombAmount);

            foreach (int cellID in randomCellsID)
            {
                PlaceBombToCell(cellID);
            }
        }

        private void PlaceBombToCell(int cellID)
        {
            var pos = Cell.GetCellPosById(cellID, boardHeight);
            var bombCell = cells[pos.x, pos.y];
            bombCell.SetCellValue(Cell.BombCellValue);

            IncreaseBombAmountToSurroundCell(bombCell.Position);
        }

        private void IncreaseBombAmountToSurroundCell(Vector2Int bombPosition)
        {
            IterateAvailableSurroundCells(bombPosition, (cell) =>
            {
                var cellValue = cell.CellValue;

                if (cellValue != Cell.BombCellValue)
                {
                    cell.SetCellValue(cellValue + score);
                }
            });
        }

        private bool IsInBound(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < boardWidth && y < boardHeight)
            {
                return true;
            }

            return false;
        }

        private void OnTriggerCell(TriggerCellMessage triggerCellMessage)
        {
            int playerId = triggerCellMessage.PlayerId;
            int cellId = triggerCellMessage.CellId;

            ReceivePlayerAction(NetworkAction.TriggerCell, playerId, cellId);
        }

        private void OnFlagCell(FlagCellMessage flagCellMessage)
        {
            int playerId = flagCellMessage.PlayerId;
            int cellId = flagCellMessage.CellId;

            ReceivePlayerAction(NetworkAction.FlagCell, playerId, cellId);
        }

        private void OnTriggerSurroundCell(TriggerSurroundCellMessage triggerSurroundCellMessage)
        {
            int playerId = triggerSurroundCellMessage.PlayerId;
            int cellId = triggerSurroundCellMessage.CenterCellId;

            ReceivePlayerAction(NetworkAction.TriggerSurroundCell, playerId, cellId);
        }

        private void OnGetSeedNumber(GetSeedNumberMessage getSeedNumberMessage)
        {
            ResetBoard(getSeedNumberMessage.SeedNumber);
            LoadingPanelManager.Instance.ShowPanel("Waiting for other players.");
        }
    }
}