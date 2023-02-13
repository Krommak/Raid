using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(BattleZoneSystem))]
public sealed class BattleZoneSystem : UpdateSystem
{
    [SerializeField]
    LayerMask unitsLayer;
    Filter activeBattleZones;
    public override void OnAwake()
    {
        var battleZones = this.World.Filter.With<BattleZoneComponent>();

        foreach (var item in battleZones)
        {
            ref var component = ref item.GetComponent<BattleZoneComponent>();
            component.EnemyUnitsEntities = new List<Entity>();
            component.PlayerUnitsEntities = new List<Entity>();
            foreach (var unit in component.Units)
            {
                component.EnemyUnitsEntities.Add(unit.Entity);
                unit.Entity.SetComponent(new UnitInBattle());

                ref var health = ref unit.Entity.GetComponent<HealthComponent>();
                health.ActualHP = health.HealthPoition;
            }

            item.SetComponent(new BattleComponent
            {
                EnemyUnitsEntities = component.EnemyUnitsEntities,
                PlayerUnitsEntities = component.PlayerUnitsEntities
            });
        }

        activeBattleZones = this.World.Filter.With<BattleZoneComponent>().Without<BattleIsDone>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in activeBattleZones)
        {
            ref var zoneComponent = ref item.GetComponent<BattleZoneComponent>();

            zoneComponent.EnemyUnitsEntities.RemoveAll(x => x.IsDisposed() || x == null);
            zoneComponent.PlayerUnitsEntities.RemoveAll(x => x.IsDisposed() || x == null);

            var zoneSize = zoneComponent.ZoneSize / 2;
            if (item.Has<BattleZoneIsActive>()) zoneSize *= 2;

            Collider[] hitColliders = Physics.OverlapBox(zoneComponent.Transform.position, zoneSize, Quaternion.identity, unitsLayer);
            foreach (var hit in hitColliders)
            {
                var playerUnit = hit.GetComponent<PlayerUnitProvider>();
                if (zoneComponent.PlayerUnitsEntities.Contains(playerUnit.Entity)) continue;
                
                zoneComponent.PlayerUnitsEntities.Add(playerUnit.Entity);
                playerUnit.Entity.SetComponent(new UnitInBattle());
            }

            if (zoneComponent.PlayerUnitsEntities.Count < 1) continue;
            
            if (!item.Has<BattleZoneIsActive>())
                item.AddComponent<BattleZoneIsActive>();

            ref var battleComponent = ref item.GetComponent<BattleComponent>();

            foreach (var playerUnitEntity in zoneComponent.PlayerUnitsEntities)
            {
                if (!battleComponent.PlayerUnitsEntities.Contains(playerUnitEntity))
                {
                    ref var battleUnitComponent = ref playerUnitEntity.GetComponent<BattleUnitComponent>();
                    battleUnitComponent.UnitRB.useGravity = false;
                    battleUnitComponent.UnitRB.velocity = Vector3.zero;

                    battleComponent.PlayerUnitsEntities.Add(playerUnitEntity);
                }
            }
        }
        foreach (var item in activeBattleZones)
        {
            ref var zone = ref item.GetComponent<BattleZoneComponent>();

            if (zone.EnemyUnitsEntities.Count == 0)
            {
                item.RemoveComponent<BattleZoneIsActive>();
                item.SetComponent(new BattleIsDone());
                foreach (var unit in zone.PlayerUnitsEntities)
                {
                    unit.RemoveComponent<UnitInBattle>();
                    ref var battleUnitComponent = ref unit.GetComponent<BattleUnitComponent>();
                    battleUnitComponent.UnitRB.useGravity = true;
                }
            }
        }
    }
}