using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.Collections.Generic;
using System.ComponentModel;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(PoolInitSystem))]
public sealed class PoolInitSystem : Initializer
{
    public override void OnAwake()
    {
        var pools = this.World.Filter.With<ObjectPoolComponent>();

        foreach (var item in pools)
        {
            ref var poolComponent = ref item.GetComponent<ObjectPoolComponent>();
            poolComponent.Units = new Queue<GameObject>();
            while (poolComponent.UnitsInPoolCount > 0)
            {
                var res = CreateUnit(poolComponent.UnitPrefab, poolComponent.Container);
                poolComponent.Units.Enqueue(res);
                poolComponent.UnitsInPoolCount--;
            }
        }
    }

    GameObject CreateUnit(GameObject prefab, Transform container)
    {
        var res = Instantiate(prefab, container);
        
        res.SetActive(false);

        return res;
    }

    public override void Dispose()
    {

    }
}