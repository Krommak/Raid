using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(FightSystem))]
public sealed class FightSystem : UpdateSystem
{
    Filter fights;
    public override void OnAwake()
    {
        fights = this.World.Filter.With<FightComponent>().With<UnitInBattle>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in fights)
        {
            ref var fightComponent = ref item.GetComponent<FightComponent>();

            if (fightComponent.Unit == null || fightComponent.TargetEnemy == null ||
                fightComponent.Unit.IsDisposed() || fightComponent.TargetEnemy.IsDisposed() ||
                !fightComponent.Unit.Has<BattleUnitComponent>() || !fightComponent.TargetEnemy.Has<BattleUnitComponent>()) continue;

            ref var unitBattleComponent = ref fightComponent.Unit.GetComponent<BattleUnitComponent>();
            ref var enemyBattleComponent = ref fightComponent.TargetEnemy.GetComponent<BattleUnitComponent>();

            var distance = (unitBattleComponent.UnitRB.position - enemyBattleComponent.UnitRB.position).magnitude;

            Vector3 enemyPosition = enemyBattleComponent.UnitRB.transform.position;

            ref var damageComponent = ref fightComponent.Unit.GetComponent<DamageComponent>();
            damageComponent.TimeForNextAttack += deltaTime;

            var unit = fightComponent.Unit;

            if (distance > damageComponent.DistanceForAttack)
            {
                unit.SetComponent(new MoveToEnemyComponent
                {
                    UnitRB = unitBattleComponent.UnitRB,
                    TargetPosition = enemyPosition
                });
                var unitTransform = fightComponent.Unit.GetComponent<RotateUnitComponent>().Transform;
                fightComponent.Unit.SetComponent(new RotateToTargetComponent
                {
                    Transform = unitTransform,
                    TargetPosition = enemyPosition
                });
            }
            else if(damageComponent.TimeForNextAttack >= damageComponent.DamageCooldown)
            {
                damageComponent.TimeForNextAttack = 0;
                fightComponent.Unit.SetComponent(new ActiveHitComponent());
                fightComponent.Unit.SetComponent(new AnimTriggerComponent()
                {
                    State = UnitAnimationState.StandingMeleeAttackDownward
                });

                unitBattleComponent.UnitRB.velocity = Vector3.zero;

                unit.SetComponent(new HitComponent
                {
                    TakingDamageUnit = fightComponent.TargetEnemy,
                    Damage = damageComponent.Damage
                });
            }
        }
    }
}