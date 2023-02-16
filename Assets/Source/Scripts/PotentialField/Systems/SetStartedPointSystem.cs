using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SetStartedPointSystem))]
public sealed class SetStartedPointSystem : UpdateSystem
{
    Filter playerUnits;
    Filter playField;
    public override void OnAwake()
    {
        playerUnits = this.World.Filter.With<PlayerUnitMovementComponent>().With<GetMeFirstPosition>();
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
                ref var getMeFirst = ref unit.GetComponent<GetMeFirstPosition>();
                movementComponent.OccupiedNode = GetNearestPosition(getMeFirst.Position, ref fieldComponent);
                if (movementComponent.OccupiedNode == Vector2.left) continue;
                else
                {
                    movementComponent.Transform.position = fieldComponent.Fields[(int)movementComponent.OccupiedNode.x, (int)movementComponent.OccupiedNode.y].Position;
                    unit.RemoveComponent<GetMeFirstPosition>();
                    unit.SetComponent(new UnitIsStay());
                }
            }
        }
    }
    Vector2 GetNearestPosition(Vector3 unitPosition, ref PlayField field)
    {
        var clampedPosition = new Vector3(
            (float)Math.Round(unitPosition.x, 1, MidpointRounding.ToEven),
             field.FirstPoint.y,
            (float)Math.Round(unitPosition.z, 1, MidpointRounding.ToEven));

        var posInArray = new Vector2(
            Mathf.Clamp((int)(clampedPosition.x / (field.NodeRadius * 2)), 0, field.Fields.GetLength(0)),
            Mathf.Clamp((int)(clampedPosition.z / (field.NodeRadius * 2)), 0, field.Fields.GetLength(1))
            );
        if (field.Fields[(int)posInArray.x, (int)posInArray.y].isAvailable)
        {
            return posInArray;
        }
        else
        {
            var xSize = field.Fields.GetLength(0);
            var zSize = field.Fields.GetLength(1);
            var xStart = Mathf.Clamp((int)posInArray.x - 1, 0, xSize);
            var zStart = Mathf.Clamp((int)posInArray.y - 1, 0, zSize);
            var xFinish = Mathf.Clamp((int)posInArray.x + 1, 0, xSize);
            var zFinish = Mathf.Clamp((int)posInArray.y + 1, 0, zSize);
            while (true)
            {
                for (int x = xStart; x <= xFinish; x++)
                {
                    for (int z = zStart; z <= zFinish; z++)
                    {
                        if (field.Fields[x, z].isAvailable)
                            return new Vector2(x, z);
                    }
                }
                xStart = Mathf.Clamp(xStart - 1, 0, xSize);
                zStart = Mathf.Clamp(zStart - 1, 0, zSize);
                xFinish = Mathf.Clamp(xFinish + 1, 0, xSize);
                zFinish = Mathf.Clamp(zFinish + 1, 0, zSize);
            }
        }
    }
}