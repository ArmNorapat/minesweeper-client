using System;
using System.Collections.Generic;
using TrueAxion.FFAMinesweepers.CellScripts;
using TrueAxion.FFAMinesweepers.Networking;
using TrueAxion.FFAMinesweepers.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrueAxion.FFAMinesweepers.Gameplay
{
    public class CellPointerManager : MonoBehaviour
    {
        public event Action<bool> InspectingSurroundCell;

        [SerializeField]
        private CellManager cellManager = default;

        [SerializeField]
        private GameplayController gameplayController = default;

        [SerializeField]
        private PlayersManager playersManager = default;

        private Vector2 currentMousePosition { get { return Input.mousePosition; } }
        private Vector2 lastMousePosition;
        private List<RaycastResult> results = new List<RaycastResult>();
        private PointerEventData pointerData = new PointerEventData(EventSystem.current);
        private Cell currentCell;
        private bool canInteractCell { get { return !gameplayController.IsGameEnd && !playersManager.IsLocalPlayerDead; } }
        private bool isMouseLeftHold { get { return Input.GetMouseButton(0); } }
        private bool isMouseRightHold { get { return Input.GetMouseButton(1); } }
        private bool isMouseLeftUp { get { return Input.GetMouseButtonUp(0); } }
        private bool isMouseRightUp { get { return Input.GetMouseButtonUp(1); } }
        private bool isMouseRightDown { get { return Input.GetMouseButtonDown(1); } }
        private bool canInspectSurroundCell = true;

        private void Start()
        {
            gameplayController.StartGame += ResetPointer;
        }

        private void Update()
        {
            if (!canInteractCell)
            {
                //Do nothing
            }
            else if (!IsAnyMouseButtonDown())
            {
                SetCanInspectSurroundCell(true);
            }
            else if (canInspectSurroundCell)
            {
                if (TryGetCellOnPointer())
                {
                    PointerOnCell();
                }
            }
        }

        private bool TryGetCellOnPointer()
        {
            pointerData.position = currentMousePosition;
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                var newCell = results[0].gameObject.GetComponent<Cell>();

                if (newCell != null)
                {
                    if (currentCell == null)
                    {
                        currentCell = newCell;
                    }

                    if (newCell != currentCell)
                    {
                        ClearInspectingCurrentCell();
                        currentCell = newCell;
                    }
                }
                else
                {
                    RemoveCurrentCell();
                }
            }
            else
            {
                RemoveCurrentCell();
            }

            return currentCell != null;
        }

        private bool IsAnyMouseButtonDown()
        {
            return isMouseRightHold || isMouseRightUp || isMouseRightDown || isMouseLeftHold || isMouseLeftUp;
        }

        private void PointerOnCell()
        {
            if (isMouseLeftHold)
            {
                if (isMouseRightUp)
                {
                    TriggerCellSurroundCurrentCell();
                }
                else
                {
                    currentCell?.InspectCell();

                    if (isMouseRightHold)
                    {
                        InspectSurroundCurrentCell();
                    }
                }
            }
            else if (isMouseRightDown)
            {
                currentCell?.TakeAction(NetworkAction.FlagCell);
            }
            else if (isMouseRightHold)
            {
                if (isMouseLeftUp)
                {
                    TriggerCellSurroundCurrentCell();
                }
            }
            else if (isMouseLeftUp)
            {
                currentCell?.TakeAction(NetworkAction.TriggerCell);
                RemoveCurrentCell();
            }
            else
            {
                ClearInspectingCurrentCell();
                SetCanInspectSurroundCell(true);
            }
        }

        private void ResetPointer()
        {
            RemoveCurrentCell();
            SetCanInspectSurroundCell(true);
        }

        private void InspectSurroundCurrentCell()
        {
            if (currentCell != null)
            {
                SetInspectSurroundCell(true);
                InspectingSurroundCell?.Invoke(true);
            }
        }

        private void ClearInspectingCurrentCell()
        {
            if (currentCell != null)
            {
                currentCell.CancelInspectCell();
                SetInspectSurroundCell(false);
                InspectingSurroundCell?.Invoke(false);
            }
        }

        private void SetInspectSurroundCell(bool isInspect)
        {
            cellManager.IterateAvailableSurroundCells(currentCell.Position, (cell) =>
            {
                if (isInspect)
                {
                    cell?.InspectCell();
                }
                else
                {
                    cell?.CancelInspectCell();
                }
            });
        }

        private void TriggerCellSurroundCurrentCell()
        {
            SetCanInspectSurroundCell(false);
            ClearInspectingCurrentCell();
            currentCell?.CancelInspectCell();
            currentCell?.TakeAction(NetworkAction.TriggerSurroundCell);
        }

        private void RemoveCurrentCell()
        {
            if (currentCell != null)
            {
                ClearInspectingCurrentCell();
                currentCell = null;
                SetCanInspectSurroundCell(true);
            }
        }

        private void SetCanInspectSurroundCell(bool value)
        {
            canInspectSurroundCell = value;
        }
    }
}