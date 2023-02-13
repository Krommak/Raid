using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct CameraMovingComponent : IComponent
{
    public Transform CameraTransform;
    public float CameraSpeed;
    public float DistanceAtFirstUnit;
    public Vector3 finalPosition;
    public Vector3 finalRotation;
}