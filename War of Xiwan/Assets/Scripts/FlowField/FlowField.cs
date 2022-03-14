using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOX.FlowField
{
    public class FlowField
    {
        public enum TerrainLayers
        {
            Impassible = 10,
            RoughTerrain = 11,

        }

        public Cell[,] Grid { get; private set; }
        public Vector3 GridStartPosition { get; private set; }
        public Vector2Int GridSize { get; private set; }
        public float CellRadius { get; private set; }

        private float cellDiameter;

        public FlowField(Vector3 gridStartPosition, Vector2Int gridSize, float cellRadius)
        {
            GridStartPosition = gridStartPosition;
            CellRadius = cellRadius;
            cellDiameter = cellRadius * 2f;
            GridSize = gridSize;
        }

        public void CreateGrid()
        {
            Grid = new Cell[GridSize.x, GridSize.y];

            for (int x = 0; x < GridSize.x; ++x)
            {
                for (int y = 0; y < GridSize.y; ++y)
                {
                    Vector3 worldPosition = GridStartPosition + new Vector3(cellDiameter * x + CellRadius, 0f, cellDiameter * y + CellRadius);
                    Grid[x, y] = new Cell(worldPosition, new Vector2Int(x, y));
                }
            }
        }

        public void CreateCostField()
        {
            Vector3 cellHalfExtents = Vector3.one * CellRadius;
            int terrainMask = LayerMask.GetMask(TerrainLayers.Impassible.ToString(), TerrainLayers.RoughTerrain.ToString());
            foreach (Cell currentCell in Grid)
            {
                Collider[] obstacles = Physics.OverlapBox(currentCell.WorldPosition, cellHalfExtents, Quaternion.identity, terrainMask);
                bool hasRoughTerrainIncreased = false;
                foreach (Collider collider in obstacles)
                {
                    if (collider.gameObject.layer == ((int)TerrainLayers.Impassible))
                    {
                        currentCell.IncreaseCost(255);
                        break;
                    }
                    else if (!hasRoughTerrainIncreased && collider.gameObject.layer == ((int)TerrainLayers.RoughTerrain))
                    {
                        currentCell.IncreaseCost(3);
                        hasRoughTerrainIncreased = true;
                    }
                }
            }
        }

        public void CreateIntegrationField(Cell destinationCell)
        {
            destinationCell.Cost = 0;
            destinationCell.BestCost = 0;

            Queue<Cell> cellsToCheck = new Queue<Cell>();
            cellsToCheck.Enqueue(destinationCell);

            while (cellsToCheck.Count > 0)
            {
                Cell currentCell = cellsToCheck.Dequeue();
                List<Cell> currentNeighbors = GetNeighborCells(currentCell.GridIndex, GridDirection.CardinalDirections);
                foreach (Cell currentNeighbor in currentNeighbors)
                {
                    if (currentNeighbor.Cost == byte.MaxValue)
                        continue;
                    if (currentNeighbor.Cost + currentCell.BestCost < currentNeighbor.BestCost)
                    {
                        currentNeighbor.BestCost = (ushort)(currentNeighbor.Cost + currentCell.BestCost);
                        cellsToCheck.Enqueue(currentNeighbor);
                    }
                }
            }
        }

        public void CreateFlowField()
        {
            foreach (Cell currentCell in Grid)
            {
                List<Cell> currentNeighbors = GetNeighborCells(currentCell.GridIndex, GridDirection.AllDirections);

                int bestCost = currentCell.BestCost;

                foreach (Cell currentNeighbor in currentNeighbors)
                {
                    if (currentCell.BestCost != ushort.MaxValue && currentNeighbor.BestCost < bestCost)
                    {
                        bestCost = currentNeighbor.BestCost;
                        currentCell.BestDirection = GridDirection.GetDirectionFromV2I(currentNeighbor.GridIndex - currentCell.GridIndex);
                    }
                }
            }
        }

        private List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
        {
            List<Cell> neighborCells = new List<Cell>();

            foreach (Vector2Int currentDirection in directions)
            {
                Cell newNeighbor = GetCellAtRelativePosition(nodeIndex, currentDirection);
                if (newNeighbor != null)
                    neighborCells.Add(newNeighbor);
            }
            return neighborCells;
        }

        private Cell GetCellAtRelativePosition(Vector2Int orignPosition, Vector2Int relativePosition)
        {
            Vector2Int finalPosition = orignPosition + relativePosition;

            if (finalPosition.x < 0
                || finalPosition.x >= GridSize.x
                || finalPosition.y < 0
                || finalPosition.y >= GridSize.y)
            {
                return null;
            }
            else
            {
                return Grid[finalPosition.x, finalPosition.y];
            }
        }

        public Cell GetCellFromWorldPosition(Vector3 worldPosition)
        {
            worldPosition = worldPosition - GridStartPosition;
            float percentX = worldPosition.x / (GridSize.x * cellDiameter);
            float percentY = worldPosition.z / (GridSize.y * cellDiameter);

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.Clamp(Mathf.FloorToInt((GridSize.x) * percentX), 0, GridSize.x - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt((GridSize.y) * percentY), 0, GridSize.y - 1);

            return Grid[x, y];
        }
    }
}
