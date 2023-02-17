using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct WeightComponent : IComponent
{
    public Transform Transform;
    public Vector2Int IncreaseWeightAndOffset;
    public Vector2Int DecreaseWeightAndOffset;

    internal Dictionary<Vector2Int, Vector2Int> InfluenceArea;
}