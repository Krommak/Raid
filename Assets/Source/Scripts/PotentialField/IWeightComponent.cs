using UnityEngine;

internal interface IWeightComponent
{
    internal Transform Transform { get; }
    internal int Weight { get; }
    internal float Offset { get; }
}