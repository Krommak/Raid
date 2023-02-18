using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System;
using Scellecs.Morpeh.Experimental;
using System.Collections.Generic;

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
        playerUnits = this.World.Filter.With<PlayerUnitMovementComponent>().With<WeightComponent>().With<UnitIsStay>().Without<GetMeFirstPosition>();
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
                ref var weightComponent = ref unit.GetComponent<WeightComponent>();

                var nextNode = GetNextPosition(movementComponent.OccupiedNode, ref fieldComponent, ref weightComponent, unit.Has<UnitInBattle>());

                if (nextNode != movementComponent.OccupiedNode)
                {
                    movementComponent.NextNode = nextNode;
                    movementComponent.TargetPosition = fieldComponent.Fields[nextNode.x, nextNode.y].Position;
                    unit.RemoveComponent<UnitIsStay>();
                }
            }
        }
    }

    Vector2Int GetNextPosition(Vector2Int unitPosition, ref PlayField field, ref WeightComponent weight, bool unitInBattle)
    {
        var potentialPos = unitPosition;
        var decreaseOffset = weight.DecreaseWeightAndOffset.y;
        var actualWeight = field.Fields[unitPosition.x, unitPosition.y].WeightForPlayer - weight.DecreaseWeightAndOffset.x;

        var xSize = field.Fields.GetLength(0) - 1;
        var zSize = field.Fields.GetLength(1) - 1;

        var firstX = Mathf.Clamp(unitPosition.x, 0, xSize);
        var firstZ = Mathf.Clamp(unitPosition.y + decreaseOffset + 2, 0, zSize);
        ref var node = ref field.Fields[firstX, firstZ];
        if (node.isAvailable && actualWeight < node.WeightForPlayer)
        {
            potentialPos = new Vector2Int(firstX, firstZ);
            actualWeight = node.WeightForPlayer;
        }
        for (int x = 0; x <= decreaseOffset + 1; x++)
        {
            var posFirst = Mathf.Clamp(firstX + x, 0, xSize);
            node = ref field.Fields[posFirst, firstZ];
            if (node.isAvailable && actualWeight < node.WeightForPlayer)
            {
                potentialPos = new Vector2Int(posFirst, firstZ);
                actualWeight = node.WeightForPlayer;
            }
            var posSecond = Mathf.Clamp(firstX - x, 0, xSize);
            node = ref field.Fields[posSecond, firstZ];
            if (node.isAvailable && actualWeight < node.WeightForPlayer)
            {
                potentialPos = new Vector2Int(posSecond, firstZ);
                actualWeight = node.WeightForPlayer;
            }
        }
        
        if(!unitInBattle) return potentialPos;

        firstX = Mathf.Clamp(unitPosition.x - (decreaseOffset + 1), 0, xSize);
        firstZ = Mathf.Clamp(unitPosition.y - (decreaseOffset + 1), 0, zSize);
        var finalX = Mathf.Clamp(unitPosition.x + (decreaseOffset + 1), 0, xSize);
        var finalZ = Mathf.Clamp(unitPosition.y + decreaseOffset, 0, zSize);
        
        for (int z = firstZ; z <= finalZ; z++)
        {
            for (int x = firstX; x <= finalX; x++)
            {
                node = ref field.Fields[x, z];
                if (node.isAvailable && actualWeight < node.WeightForPlayer)
                {
                    potentialPos = new Vector2Int(x, z);
                    actualWeight = node.WeightForPlayer;
                }
            }
        }

        return potentialPos;
    }
}