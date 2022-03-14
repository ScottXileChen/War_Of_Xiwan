using UnityEngine;

namespace WOX.FlowField
{
    public class Cell
    {
        public Vector3 WorldPosition { get; set; }
        public Vector2Int GridIndex { get; set; }
        public byte Cost { get; set; }
        public ushort BestCost { get; set; }
        public GridDirection BestDirection { get; set; }

        public Cell(Vector3 worldPosition, Vector2Int gridIndex)
        {
            WorldPosition = worldPosition;
            GridIndex = gridIndex;
            Cost = 1;
            BestCost = ushort.MaxValue;
            BestDirection = GridDirection.None;
        }

        public void IncreaseCost(int amount)
        {
            if (Cost == byte.MaxValue)
                return;
            if (amount + Cost >= 255)
                Cost = byte.MaxValue;
            else
                Cost += (byte)amount;
        }
    }
}