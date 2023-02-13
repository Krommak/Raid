using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct PlatformComponent : IComponent
{
    public PlatformType PlatformType;
    public int UnitCount;
    public Transform PlatformTransform;
    public TextMeshPro TextMesh;
}