using UnityEngine;

public struct FieldNode
{
    internal Vector3 Position;
    internal FieldNodeType NodeType;
    internal bool isAvailable;
    internal int WeightForPlayer;
    internal int WeightForEnemy;
}
