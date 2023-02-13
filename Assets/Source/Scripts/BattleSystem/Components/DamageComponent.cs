using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct DamageComponent : IComponent
{
    public float DistanceForAttack;
    public int Damage;
    public float DamageCooldown;
    [HideInInspector]
    public float TimeForNextAttack;
}