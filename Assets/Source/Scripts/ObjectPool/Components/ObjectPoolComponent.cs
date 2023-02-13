using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct ObjectPoolComponent : IComponent
{
    public GameObject UnitPrefab;
    public int UnitsInPoolCount;
    public Transform Container;
    [HideInInspector]
    public Queue<GameObject> Units;
}