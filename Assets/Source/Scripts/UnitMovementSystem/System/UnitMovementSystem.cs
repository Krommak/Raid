using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(UnitMovementSystem))]
public sealed class UnitMovementSystem : UpdateSystem
{
    [SerializeField]
    float velocityForIdle;
    Filter entities;
    public override void OnAwake()
    {
        entities = this.World.Filter.With<RotateUnitComponent>().Without<AnimTriggerComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in entities)
        {
            var rb = item.GetComponent<RotateUnitComponent>().Rigidbody;
            if (rb.velocity.z < velocityForIdle)
            {
                item.SetComponent(new AnimTriggerComponent()
                {
                    State = UnitAnimationState.StandingIdle,
                });
                continue;
            }
            item.SetComponent(new AnimTriggerComponent()
            {
                State = UnitAnimationState.StandingRunForward,
            });
        }
    }
}