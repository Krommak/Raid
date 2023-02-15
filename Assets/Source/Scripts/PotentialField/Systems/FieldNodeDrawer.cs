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
                    Gizmos.color = playField[x, z].isAvailable ? Color.white : Color.black;
                    if (playField[x, z].WeightForPlayer > 0) 
                        Gizmos.color = new Color((float)playField[x, z].WeightForPlayer/ (float)zSize, 1f - (float)playField[x, z].WeightForPlayer / (float)zSize, 1f - (float)playField[x, z].WeightForPlayer / (float)zSize);
                    Gizmos.DrawCube(playField[x, z].Position, new Vector3(0.2f, 0.2f, 0.2f));
                }
            }
        }
    }
}
