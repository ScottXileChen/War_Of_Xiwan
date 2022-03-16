using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOX.FlowField
{
    [RequireComponent(typeof(BoxCollider))]
    public class GridController : MonoBehaviour
    {
        public enum DebugDrawWay
        {
            Cost,
            BestCost,
            BestDirection
        }

        [Header("Attribute Info")]
        public Vector2 gridStartPosition = Vector2.zero;
        [Min(0)]
        public Vector2Int gridSize = Vector2Int.one;
        public float cellRadius = 0.5f;

        [Header("Debug Info")]
        [SerializeField]
        private bool debugDrawEnable = false;
        [SerializeField]
        private DebugDrawWay debugDrawWay = DebugDrawWay.BestDirection;

        public FlowField CurrentFlowField { get; private set; }

        private BoxCollider boxCollider;

        private bool initialized = false;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.size = new Vector3(gridSize.x, 0f, gridSize.y);
            boxCollider.center = boxCollider.size / 2 + new Vector3(gridStartPosition.x, 0f, gridStartPosition.y);
        }

        private void InitializeFlowField()
        {
            CurrentFlowField = new FlowField(new Vector3(gridStartPosition.x, 0f, gridStartPosition.y), gridSize, cellRadius);
            CurrentFlowField.CreateGrid();
            CurrentFlowField.CreateCostField();
        }

        private void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    InitializeFlowField();

            //    int layer = LayerMask.GetMask("FlowFied");
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(ray, out RaycastHit raycastHit, layer))
            //    {
            //        Cell dc = CurrentFlowField.GetCellFromWorldPosition(raycastHit.point);
            //        CurrentFlowField.CreateIntegrationField(dc);
            //        CurrentFlowField.CreateFlowField();
            //    }

            //    initialized = true;
            //}
        }

        //private void OnDrawGizmos()
        //{
        //    if (debugDrawEnable && initialized)
        //    {
        //        WOX.Utils.DebugRendererHelper.DrawGrid(CurrentFlowField, Color.red);
        //    }
        //}

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int x = 0; x < gridSize.x; ++x)
            {
                for (int y = 0; y < gridSize.y; ++y)
                {
                    Vector3 center = new Vector3(gridStartPosition.x, 0f, gridStartPosition.y) + new Vector3(cellRadius * 2f * x + cellRadius, 0f, cellRadius * 2f * y + cellRadius);
                    Vector3 size = Vector3.one * cellRadius * 2f;
                    size.y = 0;
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }

        private void OnGUI()
        {
            if (debugDrawEnable && initialized)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;

                foreach (Cell currentCell in CurrentFlowField.Grid)
                {
                    if (debugDrawWay == DebugDrawWay.BestDirection)
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
                    }
                    else if (debugDrawWay == DebugDrawWay.BestCost)
                    {
                        UnityEditor.Handles.Label(currentCell.WorldPosition, currentCell.BestCost.ToString(), style);
                    }
                    else if (debugDrawWay == DebugDrawWay.Cost)
                    {
                        UnityEditor.Handles.Label(currentCell.WorldPosition, currentCell.Cost.ToString(), style);
                    }
                }
            }
        }
    }
}
