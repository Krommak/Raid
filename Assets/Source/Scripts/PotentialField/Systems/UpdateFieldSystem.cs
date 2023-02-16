using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.Collections.Generic;
using System;
using static UnityEditor.Progress;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(UpdateFieldSystem))]
public sealed class UpdateFieldSystem : UpdateSystem
{
    Filter playerEntities;
    Filter enemyEntities;
    Filter playField;
    public override void OnAwake()
    {
        playerEntities = this.World.Filter.With<PlayerUnitMovementComponent>().With<UpdateField>().With<PlayerIncreaseWeightComponent>().With<PlayerDecreaseWeightComponent>();
        enemyEntities = this.World.Filter.With<PlayerUnitMovementComponent>().With<UpdateField>().With<EnemyWeightComponent>().With<EnemyWeightDecreaseComponent>();
        playField = this.World.Filter.With<PlayField>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var field in playField)
        {
            ref var fieldComponent = ref field.GetComponent<PlayField>();

            foreach (var item in playerEntities)
            {
                ref var weightComponent = ref item.GetComponent<PlayerIncreaseWeightComponent>();
                var weightInterface = (IWeightComponent)weightComponent;
                SetWeight(ref weightInterface, ref fieldComponent);
                item.RemoveComponent<UpdateField>();
            }
            foreach (var item in playerEntities)
            {
                ref var weightComponent = ref item.GetComponent<PlayerDecreaseWeightComponent>();
                var weightInterface = (IWeightComponent)weightComponent;
                SetWeight(ref weightInterface, ref fieldComponent, true);

                item.RemoveComponent<UpdateField>();
            }
            foreach (var item in enemyEntities)
            {
                ref var weightComponent = ref item.GetComponent<EnemyWeightComponent>();
                var weightInterface = (IWeightComponent)weightComponent;
                SetWeight(ref weightInterface, ref fieldComponent, false, true);
                item.RemoveComponent<UpdateField>();
            }
            foreach (var item in enemyEntities)
            {
                ref var weightComponent = ref item.GetComponent<EnemyWeightDecreaseComponent>();
                var weightInterface = (IWeightComponent)weightComponent;
                SetWeight(ref weightInterface, ref fieldComponent, true, true);

                item.RemoveComponent<UpdateField>();
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

    void SetWeight(ref IWeightComponent weightInterface, ref PlayField fieldComponent, bool isDecrease = false, bool isEnemy = false)
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

                if (!isDecrease && (isEnemy && node.WeightForPlayer > weight || !isEnemy && node.WeightForEnemy > weight)) continue;

                if (isDecrease)
                {
                    node.WeightForEnemy -= (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);
                    node.WeightForPlayer -= (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);
                }
                else if (isEnemy)
                    node.WeightForPlayer += (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);
                else
                    node.WeightForEnemy += (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);

                if (node.WeightForEnemy < 0 || node.WeightForPlayer < 0)
                {
                    node.isAvailable = false;
                }
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
}