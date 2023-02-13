using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DeathSystem))]
public sealed class DeathSystem : UpdateSystem
{
    [SerializeField]
    GameObject enemyUnitDeathEffect;
    [SerializeField]
    GameObject playerUnitDeathEffect;
    Filter deaths;
    Filter effects;
    public override void OnAwake()
    {
        deaths = this.World.Filter.With<DeathComponent>();
        effects = this.World.Filter.With<DeathEffectComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in deaths)
        {
            ArrangeAFuneral(item);
            item.RemoveComponent<DeathComponent>();
        }

        foreach (var item in effects)
        {
            ref var component = ref item.GetComponent<DeathEffectComponent>();
            component.LifeTime += deltaTime;
            if (component.LifeTime >= component.TimeForDestroy)
            {
                Destroy(component.GameObject);
            }
        }
    }

    void ArrangeAFuneral(Entity entity)
    {
        ref var death = ref entity.GetComponent<DeathComponent>();
        var isPlayerUnit = death.DeathUnit.Has<PlayerUnitComponent>();
        var pos = death.UnitGO.transform.position;

        if (!isPlayerUnit)
        {
            Instantiate(enemyUnitDeathEffect, pos, Quaternion.identity);
            death.DeathUnit.Dispose();
            death.UnitGO.SetActive(false);
            return;
        }

        Instantiate(playerUnitDeathEffect, pos, Quaternion.identity);
        death.DeathUnit.SetComponent(new ReturnToPoolComponent
        {
            ReturnedUnit = death.DeathUnit,
            UnitGO = death.UnitGO,
        });
    }
}