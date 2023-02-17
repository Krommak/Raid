using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct EnemyWeightComponent : IComponent
{
    public Transform Transform;
    public Vector2Int IncreaseWeightAndOffset;
    public Vector2Int DecreaseWeightAndOffset;

    internal Dictionary<Vector2Int, KeyValuePair<int, int>> InfluenceArea;
}