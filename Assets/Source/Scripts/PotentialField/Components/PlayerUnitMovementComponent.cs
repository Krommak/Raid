using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct PlayerUnitMovementComponent : IComponent, IDisposable
{
    public Transform Transform;
    public float Speed;
    internal Vector3 TargetPosition;
    internal Vector2 OccupiedNode;

    public void Dispose()
    {
        OccupiedNode = Vector2.left;
    }
}