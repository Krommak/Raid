using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct PlayerDecreaseWeightComponent : IComponent, IWeightComponent
{
    [SerializeField]
    Transform Transform;
    [SerializeField]
    int Weight;
    [SerializeField]
    int Offset;

    Transform IWeightComponent.Transform { get => this.Transform; }
    int IWeightComponent.Weight { get => this.Weight; }
    int IWeightComponent.Offset { get => this.Offset; }
}