using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOX.FlowField
{
    public class GridController : MonoBehaviour
    {
        [SerializeField]
        private Camera lookCamera = null;
        public Vector2 gridStartPosition = Vector2.zero;
        [Min(0)]
        public Vector2Int gridSize = Vector2Int.one;
        public float cellRadius = 0.5f;
        public FlowField CurrentFlowField { get; private set; }

        private bool initialized = false;

        private void InitializeFlowField()
        {
            CurrentFlowField = new FlowField(new Vector3(gridStartPosition.x, 0f, gridStartPosition.y), gridSize, cellRadius);
            CurrentFlowField.CreateGrid();
            CurrentFlowField.CreateCostField();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                InitializeFlowField();

                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
                Vector3 worldMousePosition = lookCamera.ScreenToWorldPoint(mousePosition);

                Cell dc = CurrentFlowField.GetCellFromWorldPosition(worldMousePosition);
                CurrentFlowField.CreateIntegrationField(dc);
                CurrentFlowField.CreateFlowField();

                initialized = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (initialized)
            {
                WOX.Utils.DebugRendererHelper.DrawGrid(CurrentFlowField, Color.red);
            }
        }

        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;

            if (initialized)
            {
                foreach (Cell currentCell in CurrentFlowField.Grid)
                {
                    if (currentCell.BestDirection == GridDirection.None)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "X", style);
                    else if (currentCell.BestDirection == GridDirection.North)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¡ü", style);
                    else if (currentCell.BestDirection == GridDirection.South)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¡ý", style);
                    else if (currentCell.BestDirection == GridDirection.East)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¡ú", style);
                    else if (currentCell.BestDirection == GridDirection.West)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¡û", style);
                    else if (currentCell.BestDirection == GridDirection.NorthEast)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¨J", style);
                    else if (currentCell.BestDirection == GridDirection.NorthWest)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¨I", style);
                    else if (currentCell.BestDirection == GridDirection.SouthEast)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¨K", style);
                    else if (currentCell.BestDirection == GridDirection.SouthWest)
                        UnityEditor.Handles.Label(currentCell.WorldPosition, "¨L", style);

                    //UnityEditor.Handles.Label(currentCell.WorldPosition, currentCell.BestCost.ToString(), style);

                }
            }
        }
    }
}
