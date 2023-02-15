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
                var clampedLastPosition = new Vector3(
                    (float)Math.Round(transform.position.x + weightComponent.Offset, 1, MidpointRounding.ToEven),
                     fieldComponent.FirstPoint.y,
                    (float)Math.Round(transform.position.z + weightComponent.Offset, 1, MidpointRounding.ToEven));

                Vector2 start = new Vector2(
                    Mathf.Clamp((int)(clampedFirstPosition.x / (fieldComponent.NodeRadius * 2)), 0, xSize),
                    Mathf.Clamp((int)(clampedFirstPosition.z / (fieldComponent.NodeRadius * 2)), 0, zSize)
                    );
                Vector2 final = new Vector2(
                    Mathf.Clamp((int)(clampedLastPosition.x / (fieldComponent.NodeRadius * 2)), 0, xSize),
                    Mathf.Clamp((int)(clampedLastPosition.z / (fieldComponent.NodeRadius * 2)), 0, zSize)
                    );

                for (int x = (int)start.x; x < (int)final.x; x++)
                {
                    for (int z = (int)start.y; z < (int)final.y; z++)
                    {
                        ref var node = ref fieldComponent.Fields[x,z];

                        if (!node.isAvailable || node.WeightForPlayer > weightComponent.Weight) continue;

                        node.WeightForPlayer += (int)Mathf.Lerp(weightComponent.Weight, 1, (transform.position - node.Position).magnitude);
                    }
                }
            }
            foreach (var item in finishEntity)
            {
                ref var weightComponent = ref item.GetComponent<FinishWeightComponent>();
                
                for (int x = 0; x < xSize; x++)
                {
                    for (int z = 0; z < zSize; z++)
                    {
                        ref var node = ref fieldComponent.Fields[x, z];
                        
                        if (!node.isAvailable) continue;

                        node.WeightForPlayer += z;
                    }
                }
            }
        }
    }

    public override void Dispose()
    {
    }
}