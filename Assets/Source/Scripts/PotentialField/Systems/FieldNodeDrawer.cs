using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FieldNodeDrawer : MonoBehaviour
{
    bool draw = false;
    FieldNode[,] playField;

    public void DrawNodes(FieldNode[,] nodes)
    {
        draw = true;
        playField = nodes;
    }

    private void OnDrawGizmos()
    {
        if (draw)
        {
            var xSize = playField.GetLength(0);
            var zSize= playField.GetLength(1);
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    Gizmos.color = playField[x, z].isAvailable ? Color.green : Color.red;
                    if (playField[x, z].WeightForPlayer > 0) Gizmos.color = new Color(playField[x, z].WeightForPlayer*2, 0, 0);
                    Gizmos.DrawCube(playField[x, z].Position, new Vector3(0.05f, 0.05f, 0.05f));
                }
            }
        }
    }
}
