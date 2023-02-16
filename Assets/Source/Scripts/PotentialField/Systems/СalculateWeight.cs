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
        enemyEntity = this.World.Filter.With<EnemyWeightComponent>().With<EnemyWeightDecreaseComponent>();
        playField = this.World.Filter.With<PlayField>();
        foreach (var field in playField)
        {
            ref var fieldComponent = ref field.GetComponent<PlayField>();

            foreach (var item in enemyEntity)
            {
                ref var weightComponent = ref item.GetComponent<EnemyWeightComponent>();
                var weightInterface = (IWeightComponent)weightComponent;
                SetWeight(ref weightInterface, ref fieldComponent);
            }
            foreach (var item in enemyEntity)
            {
                ref var weightComponent = ref item.GetComponent<EnemyWeightDecreaseComponent>();
                var weightInterface = (IWeightComponent)weightComponent;
                SetWeight(ref weightInterface, ref fieldComponent, true);
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

    void SetWeight(ref IWeightComponent weightInterface, ref PlayField fieldComponent, bool isDecrease = false)
    {
        var center = GetCenter(ref weightInterface, ref fieldComponent);

        var weight = weightInterface.Weight;

        var xSize = fieldComponent.Fields.GetLength(0);
        var zSize = fieldComponent.Fields.GetLength(1);
        var xStart = Mathf.Clamp((int)center.x - weightInterface.Offset, 0, xSize);
        var zStart = Mathf.Clamp((int)center.y - weightInterface.Offset, 0, zSize);
        var xFinish = Mathf.Clamp((int)center.x + weightInterface.Offset, 0, xSize);
        var zFinish = Mathf.Clamp((int)center.y + weightInterface.Offset, 0, zSize);
        for (int x = xStart; x <= xFinish; x++)
        {
            for (int z = zStart; z <= zFinish; z++)
            {
                ref var node = ref fieldComponent.Fields[x, z];

                if (!isDecrease && (node.WeightForPlayer > weight || node.WeightForEnemy > weight)) continue;

                node.WeightForPlayer += (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);

                if (isDecrease)
                {
                    node.WeightForEnemy -= (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);
                    node.WeightForPlayer -= (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);
                }

                if (node.WeightForEnemy < 0 || node.WeightForPlayer < 0)
                    node.isAvailable = false;
                else
                    node.isAvailable = true;
            }
        }
    }

    Vector2 GetCenter(ref IWeightComponent weightComponent, ref PlayField fieldComponent)
    {
        var xSize = fieldComponent.Fields.GetLength(0);
        var zSize = fieldComponent.Fields.GetLength(1);
        var transform = weightComponent.Transform;

        var centerPosition = new Vector2(
            (float)Math.Round(transform.position.x, 1, MidpointRounding.ToEven),
            (float)Math.Round(transform.position.z, 1, MidpointRounding.ToEven));

        return new Vector2(
            Mathf.Clamp((int)(centerPosition.x / (fieldComponent.NodeRadius * 2)), 0, xSize),
            Mathf.Clamp((int)(centerPosition.y / (fieldComponent.NodeRadius * 2)), 0, zSize)
            );
    }

    public override void Dispose()
    {
    }
}