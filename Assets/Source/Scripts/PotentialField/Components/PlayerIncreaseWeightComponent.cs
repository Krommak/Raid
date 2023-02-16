using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct PlayerIncreaseWeightComponent : IComponent, IWeightComponent
{
    [SerializeField]
    Transform Transform;
    [SerializeField]
    int Weight;
    [SerializeField]
    float Offset;

    Transform IWeightComponent.Transform { get => this.Transform; }
    int IWeightComponent.Weight { get => this.Weight; }
    float IWeightComponent.Offset { get => this.Offset; }
}