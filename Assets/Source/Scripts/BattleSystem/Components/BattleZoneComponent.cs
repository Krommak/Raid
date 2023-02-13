using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct BattleZoneComponent : IComponent
{
    public List<EnemyUnitProvider> Units;
    public Transform Transform;
    public Vector3 ZoneSize;
    internal List<Entity> PlayerUnitsEntities;
    internal List<Entity> EnemyUnitsEntities;
}