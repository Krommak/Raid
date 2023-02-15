using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(СalculateWeight))]
public sealed class СalculateWeight : Initializer
{
    Filter finishEntity;
    Filter enemyEntity;
    Filter playField;
    public override void OnAwake()
    {
        finishEntity = this.World.Filter.With<FinishWeightComponent>();
        enemyEntity = this.World.Filter.With<EnemyWeightComponent>();
        playField = this.World.Filter.With<PlayField>();
        foreach (var field in playField)
        {
            ref var fieldComponent = ref field.GetComponent<PlayField>();

            var xSize = fieldComponent.Fields.GetLength(0);
            var zSize = fieldComponent.Fields.GetLength(1);
            foreach (var item in enemyEntity)
            {
                ref var weightComponent = ref item.GetComponent<EnemyWeightComponent>();
                var transform = weightComponent.Transform;
                var clampedFirstPosition = new Vector3(
                    (float)Math.Round(transform.position.x - weightComponent.Offset, 1, MidpointRounding.ToEven),
                     fieldComponent.FirstPoint.y,
                    (float)Math.Round(transform.position.z - weightComponent.Offset, 1, MidpointRounding.ToEven));

                Vector2 start = new Vector2(
                    (int)(clampedFirstPosition.x / (fieldComponent.NodeRadius * 2)),
                    (int)(clampedFirstPosition.z / (fieldComponent.NodeRadius * 2))
                    );
                var countSqrt = (int)(weightComponent.Offset*2 / fieldComponent.NodeRadius);
                Vector2 final = new Vector2(
                    start.x + countSqrt, 
                    start.y + countSqrt
                    );

                for (int x = (int)start.x; x < (int)final.x; x++)
                {
                    for (int z = (int)start.y; z < (int)final.y; z++)
                    {
                        ref var node = ref fieldComponent.Fields[x,z];
                        if (node.WeightForPlayer > weightComponent.Weight) continue;

                        node.WeightForPlayer += (int)Mathf.Lerp(weightComponent.Weight, 1, (transform.position - node.Position).magnitude);
                    }
                }
            }
        }
    }

    public override void Dispose()
    {
    }
}