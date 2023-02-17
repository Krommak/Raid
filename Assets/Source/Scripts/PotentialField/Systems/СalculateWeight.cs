using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

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
        enemyEntity = this.World.Filter.With<EnemyWeightComponent>().With<WeightComponent>();
        playField = this.World.Filter.With<PlayField>();
        foreach (var field in playField)
        {
            ref var fieldComponent = ref field.GetComponent<PlayField>();

            foreach (var item in enemyEntity)
            {
                CalculateWeight(item, ref fieldComponent);
            }
            foreach (var item in finishEntity)
            {
                ref var weightComponent = ref item.GetComponent<FinishWeightComponent>();
                var xSize = fieldComponent.Fields.GetLength(0);
                var zSize = fieldComponent.Fields.GetLength(1);
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

            //Test
            var draw = this.World.Filter.With<FiledDrawerComponent>();

            foreach (var item in draw)
            {
                ref var comp = ref item.GetComponent<FiledDrawerComponent>();
                comp.Drawer.DrawNodes(fieldComponent.Fields);
            }
        }
    }

    void CalculateWeight(Entity entity, ref PlayField fieldComponent)
    {
        ref WeightComponent weightComponent = ref entity.GetComponent<WeightComponent>();
        SetWeight(ref weightComponent, ref fieldComponent);
    }

    void SetWeight(ref WeightComponent weightComponent, ref PlayField fieldComponent)
    {
        var center = GetCenter(weightComponent.Transform.position, ref fieldComponent);

        fieldComponent.Fields[center.x, center.y].isAvailable = false;

        SetWeight(center, weightComponent.IncreaseWeightAndOffset, ref fieldComponent, false);
        SetWeight(center, weightComponent.DecreaseWeightAndOffset, ref fieldComponent, true);
    }

    void SetWeight(Vector2Int center, Vector2Int weightAndOffset, ref PlayField fieldComponent, bool isDecrease)
    {
        var xSize = fieldComponent.Fields.GetLength(0) - 1;
        var zSize = fieldComponent.Fields.GetLength(1) - 1;
        var xStart = Mathf.Clamp(center.x - weightAndOffset.y, 0, xSize);
        var zStart = Mathf.Clamp(center.y - weightAndOffset.y, 0, zSize);
        var xFinish = Mathf.Clamp(center.x + weightAndOffset.y, 0, xSize);
        var zFinish = Mathf.Clamp(center.y + weightAndOffset.y, 0, zSize);
        for (int x = xStart; x <= xFinish; x++)
        {
            for (int z = zStart; z <= zFinish; z++)
            {
                ref var node = ref fieldComponent.Fields[x, z];

                var influence = weightAndOffset.x / (Math.Abs(center.x - x + center.y - z) + 1);

                if (isDecrease)
                {
                    node.WeightForEnemy += influence;
                    node.WeightForPlayer += influence;
                }
                else
                    node.WeightForPlayer += influence;
            }
        }
    }

    Vector2Int GetCenter(Vector3 pos, ref PlayField fieldComponent)
    {
        var xSize = fieldComponent.Fields.GetLength(0);
        var zSize = fieldComponent.Fields.GetLength(1);
        var centerPosition = new Vector2(
            (float)Math.Round(pos.x, 1, MidpointRounding.ToEven),
            (float)Math.Round(pos.z, 1, MidpointRounding.ToEven));

        return new Vector2Int(
            Mathf.Clamp((int)(centerPosition.x / (fieldComponent.NodeRadius * 2)), 0, xSize),
            Mathf.Clamp((int)(centerPosition.y / (fieldComponent.NodeRadius * 2)), 0, zSize)
            );
    }

    public override void Dispose()
    {
    }
}