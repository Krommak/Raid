using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct BattleComponent : IComponent
{
    [HideInInspector]
    public List<Entity> PlayerUnitsEntities;
    [HideInInspector]
    public List<Entity> EnemyUnitsEntities;
}