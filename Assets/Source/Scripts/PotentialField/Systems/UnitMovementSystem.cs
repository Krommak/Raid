using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using TMPro;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(UnitMovementSystem))]
public sealed class UnitMovementSystem : UpdateSystem
{
    [SerializeField]
    float distanceToTargetForStop;
    Filter playerUnits;
    public override void OnAwake()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        playerUnits = this.World.Filter.With<PlayerUnitMovementComponent>().Without<UnitIsStay>().Without<GetMeFirstPosition>();
        foreach (var unit in playerUnits)
        {
            ref var movementComponent = ref unit.GetComponent<PlayerUnitMovementComponent>();

            movementComponent.Transform.position = Vector3.MoveTowards(movementComponent.Transform.position, movementComponent.TargetPosition, movementComponent.Speed* deltaTime);

            if ((movementComponent.Transform.position - movementComponent.TargetPosition).magnitude <= distanceToTargetForStop)
            {
                movementComponent.Transform.position = movementComponent.TargetPosition;
                unit.SetComponent(new UpdateField()
                {
                    UpdateWithReset = true,
                });
            }
        }
    }
}