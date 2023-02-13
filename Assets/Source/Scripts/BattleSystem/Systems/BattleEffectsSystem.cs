using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(BattleEffectsSystem))]
public sealed class BattleEffectsSystem : UpdateSystem
{
    Filter trails;
    Filter damageEffects;
    public override void OnAwake()
    {
        trails = this.World.Filter.With<HitEffectComponent>().With<ActiveHitComponent>();
        damageEffects = this.World.Filter.With<DamageEffectComponent>().With<ActiveDamageComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in trails)
        {
            ref var hit = ref item.GetComponent<HitEffectComponent>();

            if (hit.Trail != null) continue;
            if (hit.Trail.enabled == true)
            {
                hit.TimeForDisable -= deltaTime;
                if (hit.TimeForDisable <= 0)
                {
                    hit.Trail.enabled = false;
                    item.RemoveComponent<ActiveHitComponent>();
                }
            }
            else
            {
                hit.Trail.enabled = true;
                hit.TimeForDisable = hit.LifeTime;
            }
        }

        foreach (var item in damageEffects)
        {
            ref var damageEffect = ref item.GetComponent<DamageEffectComponent>();

            damageEffect.Particle.Play();
            item.RemoveComponent<ActiveDamageComponent>();
        }
    }
}