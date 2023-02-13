using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.ComponentModel;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(HitSystem))]
public sealed class HitSystem : UpdateSystem
{
    Filter hits;
    public override void OnAwake()
    {
        hits = this.World.Filter.With<HitComponent>().With<ActiveHitComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in hits)
        {
            ref var hit = ref item.GetComponent<HitComponent>();

            if (hit.TakingDamageUnit == null || !hit.TakingDamageUnit.Has<HealthComponent>()) continue;

            hit.TakingDamageUnit.SetComponent(new ActiveDamageComponent());

            ref var healthComponent = ref hit.TakingDamageUnit.GetComponent<HealthComponent>();
            healthComponent.ActualHP -= hit.Damage;
            if(healthComponent.ActualHP <= 0)
            {
                hit.TakingDamageUnit.RemoveComponent<UnitInBattle>();
                ref var battleUnitComponent = ref hit.TakingDamageUnit.GetComponent<BattleUnitComponent>();

                battleUnitComponent.UnitRB.useGravity = true;
                healthComponent.ActualHP = healthComponent.HealthPoition;

                var death = this.World.CreateEntity();
                death.SetComponent(new DeathComponent
                {
                    DeathUnit = hit.TakingDamageUnit,
                    UnitGO = battleUnitComponent.UnitRB.gameObject
                });
            }

            item.RemoveComponent<ActiveHitComponent>();
        }
    }
}