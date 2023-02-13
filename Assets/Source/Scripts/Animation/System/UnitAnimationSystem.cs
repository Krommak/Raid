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
        entities = this.World.Filter.With<UnitAnimatorComponent>().With<AnimTriggerComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in entities)
        {
            ref var animator = ref item.GetComponent<UnitAnimatorComponent>();
            ref var trigger = ref item.GetComponent<AnimTriggerComponent>();

            var state = trigger.State;

            if (state == animator.CurrentState) continue;

            animator.Animator.SetInteger("Action", state.GetHashCode());
            animator.CurrentState = state;

            item.RemoveComponent<AnimTriggerComponent>();
            if (state != UnitAnimationState.Idle)
                item.SetComponent(new AnimTriggerComponent()
                {
                    State = UnitAnimationState.Idle,
                });
        }
    }
}