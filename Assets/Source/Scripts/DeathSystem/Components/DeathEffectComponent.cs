using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct DeathEffectComponent : IComponent
{
    public float TimeForDestroy;
    public GameObject GameObject;
    [HideInInspector]
    public float LifeTime;
}