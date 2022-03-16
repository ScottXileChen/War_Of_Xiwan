using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOX.Utils
{
    public static class DebugRendererHelper
    {
        public static void DrawGrid(FlowField.FlowField flowField, Color drawColor)
        {
            Gizmos.color = drawColor;
            for (int x = 0; x < flowField.GridSize.x; ++x)
            {
                for (int y = 0; y < flowField.GridSize.y; ++y)
                {
                    Vector3 center = flowField.GridStartPosition + new Vector3(flowField.CellRadius * 2f * x + flowField.CellRadius, 0f, flowField.CellRadius * 2f * y + flowField.CellRadius);
                    Vector3 size = Vector3.one * flowField.CellRadius * 2f;
                    size.y = 0;
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    }
}
