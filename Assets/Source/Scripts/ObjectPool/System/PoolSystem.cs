using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using UnityEngine.UIElements;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PoolSystem))]
public sealed class PoolSystem : UpdateSystem
{
    Filter pools;
    public override void OnAwake()
    {
        pools = this.World.Filter.With<ObjectPoolComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (pools.IsEmpty()) return;

        GetUnits(deltaTime);
        ReturnUnit();
    }

    void ReturnUnit()
    {
        var returnUnits = this.World.Filter.With<ReturnToPoolComponent>();

        if (returnUnits.IsEmpty()) return;

        ObjectPoolComponent objectPoolComponent = default;

        foreach (var item in pools)
        {
            objectPoolComponent = item.GetComponent<ObjectPoolComponent>();
        }

        foreach (var item in returnUnits)
        {
            ref var component = ref item.GetComponent<ReturnToPoolComponent>();

            objectPoolComponent.Units.Enqueue(component.UnitGO);
            component.UnitGO.SetActive(false);
            item.RemoveComponent<ReturnToPoolComponent>();
            item.Dispose();

            var decrease = this.World.CreateEntity();
            decrease.SetComponent(new DecreaseScoreComponent());
        }
    }

    void GetUnits(float deltaTime)
    {
        var getUnits = this.World.Filter.With<GetUnitComponent>();

        if (getUnits.IsEmpty()) return;

        ObjectPoolComponent objectPoolComponent = default;

        foreach (var item in pools)
        {
            objectPoolComponent = item.GetComponent<ObjectPoolComponent>();
        }

        foreach (var item in getUnits)
        {
            ref var component = ref item.GetComponent<GetUnitComponent>();
            component.RespawnProgress += deltaTime;

            if (component.RespawnProgress >= component.TimeForRespawn)
            {
                component.RespawnProgress = 0;
                if (objectPoolComponent.Units.Count == 0)
                {
                    objectPoolComponent.Units.Enqueue(CreateUnit(objectPoolComponent.UnitPrefab, objectPoolComponent.Container));
                }

                var res = objectPoolComponent.Units.Dequeue();
                res.SetActive(true);

                var entity = res.GetComponent<HealthProvider>().Entity;
                ref var health = ref entity.GetComponent<HealthComponent>();
                health.ActualHP = health.HealthPoition;
                entity.SetComponent(new GetMeFirstPosition()
                {
                    Position = component.Position
                });
                entity.SetComponent(new UnitIsStay());
                component.UnitsCount--;

                item.SetComponent(new IncreaseScoreComponent());
            }

            if (component.UnitsCount == 0)
                item.RemoveComponent<GetUnitComponent>();
        }
    }

    GameObject CreateUnit(GameObject prefab, Transform container)
    {
        var res = Instantiate(prefab, container);

        return res;
    }
}