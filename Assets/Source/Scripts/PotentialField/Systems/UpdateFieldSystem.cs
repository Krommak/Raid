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
        var quad = GetQuad(ref weightInterface, ref fieldComponent);

        var weight = weightInterface.Weight;
        
        Vector2 start = quad.Key;
        Vector2 final = quad.Value;

        for (int x = (int)start.x; x < (int)final.x; x++)
        {
            for (int z = (int)start.y; z < (int)final.y; z++)
            {
                ref var node = ref fieldComponent.Fields[x, z];

                if (!isDecrease && (isEnemy && node.WeightForPlayer > weight || !isEnemy && node.WeightForEnemy > weight)) continue;

                if (isDecrease)
                {
                    node.WeightForEnemy += (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);
                    node.WeightForPlayer += (int)Mathf.Lerp(weight, 1, (weightInterface.Transform.position - node.Position).magnitude);
                }
                else if(isEnemy)
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

    KeyValuePair<Vector2, Vector2> GetQuad(ref IWeightComponent weightComponent, ref PlayField fieldComponent)
    {
        var xSize = fieldComponent.Fields.GetLength(0);
        var zSize = fieldComponent.Fields.GetLength(1);
        var transform = weightComponent.Transform;

        var clampedFirstPosition = new Vector3(
            (float)Math.Round(transform.position.x - weightComponent.Offset, 1, MidpointRounding.ToEven),
             fieldComponent.FirstPoint.y,
            (float)Math.Round(transform.position.z - weightComponent.Offset, 1, MidpointRounding.ToEven));

        var clampedLastPosition = new Vector3(
            (float)Math.Round(transform.position.x + weightComponent.Offset, 1, MidpointRounding.ToEven),
             fieldComponent.FirstPoint.y,
            (float)Math.Round(transform.position.z + weightComponent.Offset, 1, MidpointRounding.ToEven));

        return new KeyValuePair<Vector2, Vector2>(
            new Vector2(
            Mathf.Clamp((int)(clampedFirstPosition.x / (fieldComponent.NodeRadius * 2)), 0, xSize),
            Mathf.Clamp((int)(clampedFirstPosition.z / (fieldComponent.NodeRadius * 2)), 0, zSize)),
            new Vector2(
            Mathf.Clamp((int)(clampedLastPosition.x / (fieldComponent.NodeRadius * 2)), 0, xSize),
            Mathf.Clamp((int)(clampedLastPosition.z / (fieldComponent.NodeRadius * 2)), 0, zSize)));
    }
}