using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct RotateUnitComponent : IComponent
{
    [SerializeField]
    public Rigidbody Rigidbody;
    [SerializeField]
    public Transform Transform;
}