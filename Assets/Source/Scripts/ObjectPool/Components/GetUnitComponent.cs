using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct GetUnitComponent : IComponent
{
    public float TimeForRespawn;
    [HideInInspector]
    public float RespawnProgress;
    public int UnitsCount;
    public Vector3 Position;
}