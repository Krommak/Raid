using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(RotateUnitSystem))]
public sealed class RotateUnitSystem : UpdateSystem
{
    Filter allInBattle;
    Filter playerEntities;

    public override void OnAwake()
    {
        var enemyEntities = this.World.Filter.With<RotateUnitComponent>().With<EnemyUnitComponent>().Without<UnitInBattle>();
        foreach (var item in enemyEntities)
        {
            ref var component = ref item.GetComponent<RotateUnitComponent>();
            var transform = component.Transform;
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        allInBattle = this.World.Filter.With<RotateToTargetComponent>().With<UnitInBattle>();
        playerEntities = this.World.Filter.With<RotateUnitComponent>().With<PlayerUnitComponent>().Without<UnitInBattle>();
    }

    public override void OnUpdate(float deltaTime)
    {
        RotateUnits(true);

        foreach (var item in allInBattle)
        {
            ref var component = ref item.GetComponent<RotateToTargetComponent>();

            component.Transform.LookAt(component.TargetPosition);
            item.RemoveComponent<RotateToTargetComponent>();
        }
    }

    void RotateUnits(bool isPlayerUnits)
    {
        foreach (var item in playerEntities)
        {
            ref var component = ref item.GetComponent<RotateUnitComponent>();
            var transform = component.Transform;
            var rb = component.Rigidbody;

            if (rb.IsSleeping()) continue;

            var angle = isPlayerUnits ? Vector3.Angle(rb.velocity, Vector3.forward) * rb.velocity.x : Vector3.Angle(rb.velocity, Vector3.forward);
            transform.eulerAngles = new Vector3(0, angle, 0);
        }
    }
}