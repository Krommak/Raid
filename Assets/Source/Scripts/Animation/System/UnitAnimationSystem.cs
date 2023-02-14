using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(UnitAnimationSystem))]
public sealed class UnitAnimationSystem : UpdateSystem
{
    Filter entities;
    public override void OnAwake()
    {
        var initEntities = this.World.Filter.With<UnitAnimatorComponent>()/*.With<AnimTriggerComponent>()*/;

        foreach (var item in initEntities)
        {
            ref var component = ref item.GetComponent<UnitAnimatorComponent>();
            component.CurrentState = UnitAnimationState.StandingIdle;
            item.SetComponent(new AnimTriggerComponent
            {
                State = UnitAnimationState.StandingIdle
            });
        }
        entities = this.World.Filter.With<UnitAnimatorComponent>().With<AnimTriggerComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in entities)
        {
            ref var animator = ref item.GetComponent<UnitAnimatorComponent>();
            ref var trigger = ref item.GetComponent<AnimTriggerComponent>();

            var state = trigger.State;

            if (state == animator.CurrentState || animator.Animator == null) continue;

            animator.Animator.Play(state.ToString());
            animator.CurrentState = state;

            item.RemoveComponent<AnimTriggerComponent>();
            if (state != UnitAnimationState.StandingIdle)
                item.SetComponent(new AnimTriggerComponent()
                {
                    State = UnitAnimationState.StandingIdle,
                });
        }
    }
}