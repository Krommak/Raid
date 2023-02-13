using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(BattleSystem))]
public sealed class BattleSystem : UpdateSystem
{
    Filter battles;
    public override void OnAwake()
    {
        battles = this.World.Filter.With<BattleComponent>().With<BattleZoneIsActive>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in battles)
        {
            ref var battleComponent = ref item.GetComponent<BattleComponent>();

            List<Entity> playerUnits = battleComponent.PlayerUnitsEntities;
            List<Entity> enemyUnits = battleComponent.EnemyUnitsEntities;

            CreateFights(playerUnits, enemyUnits);
            CreateFights(enemyUnits, playerUnits);
        }
    }

    void CreateFights(List<Entity> units, List<Entity> targets)
    {
        for (int x = 0; x < units.Count; x++)
        {
            if (units[x].IsDisposed() || units[x] == null) continue;

            if (!units[x].Has<FightComponent>())
            {
                units[x].SetComponent(new FightComponent()
                {
                    Unit = units[x]
                });
            }
            ref var fight = ref units[x].GetComponent<FightComponent>();

            if(!units[x].Has<BattleUnitComponent>()) continue;
            Vector3 unitPosition = units[x].GetComponent<BattleUnitComponent>().UnitRB.position;

            float minDistance = 1000;
            foreach (var element in targets)
            {
                if (element.IsDisposed() || !element.Has<BattleUnitComponent>() || element == null) continue;

                Vector3 targetPosition = element.GetComponent<BattleUnitComponent>().UnitRB.position;
                var distance = Vector3.Distance(unitPosition, targetPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    fight.TargetEnemy = element;
                }
            }
        }
    }
}