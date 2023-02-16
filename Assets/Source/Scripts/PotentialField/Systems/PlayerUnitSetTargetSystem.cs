using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System;
using Scellecs.Morpeh.Experimental;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerUnitSetTargetSystem))]
public sealed class PlayerUnitSetTargetSystem : UpdateSystem
{
    Filter playerUnits;
    Filter playField;
    public override void OnAwake()
    {
        playerUnits = this.World.Filter.With<PlayerUnitMovementComponent>().With<UnitIsStay>().Without<GetMeFirstPosition>();
        playField = this.World.Filter.With<PlayField>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var field in playField)
        {
            ref var fieldComponent = ref field.GetComponent<PlayField>();
            foreach (var unit in playerUnits)
            {
                ref var movementComponent = ref unit.GetComponent<PlayerUnitMovementComponent>();

                if (movementComponent.OccupiedNode == Vector2.left)
                {
                    unit.SetComponent(new GetMeFirstPosition());
                    continue;
                }

                movementComponent.TargetPosition = GetTargetPosition(movementComponent.Transform.position, ref fieldComponent);
                unit.RemoveComponent<UnitIsStay>();
                unit.SetComponent(new UpdateField());
            }
        }
    }

    Vector3 GetTargetPosition(Vector3 unitPosition, ref PlayField field)
    {
        var clampedPosition = new Vector3(
            (float)Math.Round(unitPosition.x, 1, MidpointRounding.ToEven),
             field.FirstPoint.y,
            (float)Math.Round(unitPosition.z, 1, MidpointRounding.ToEven));

        var posInArray = new Vector2(
            Mathf.Clamp((int)(clampedPosition.x / (field.NodeRadius * 2)), 0, field.Fields.GetLength(0)),
            Mathf.Clamp((int)(clampedPosition.z / (field.NodeRadius * 2)), 0, field.Fields.GetLength(1))
            );


        var firstPos = new Vector2(
                Mathf.Clamp((int)posInArray.x - 1, 0, field.Fields.GetLength(0)),
                Mathf.Clamp((int)posInArray.y - 1, 0, field.Fields.GetLength(1)));

        var lastPos = new Vector2(
                Mathf.Clamp((int)posInArray.x - 1, 0, field.Fields.GetLength(0)),
                Mathf.Clamp((int)posInArray.y - 1, 0, field.Fields.GetLength(1)));

        var actualWeight = field.Fields[(int)posInArray.x, (int)posInArray.y].WeightForPlayer;

        var potentialPos = unitPosition;
        for (int x = (int)firstPos.x; x <= (int)lastPos.x; x++)
        {
            for (int z = (int)firstPos.y; z <= (int)lastPos.y; z++)
            {
                if (field.Fields[x, z].WeightForPlayer > actualWeight)
                    potentialPos = field.Fields[x, z].Position;
            }
        }

        return potentialPos;
    }
}