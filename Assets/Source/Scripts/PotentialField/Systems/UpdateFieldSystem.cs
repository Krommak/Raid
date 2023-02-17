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
        playerEntities = this.World.Filter.With<PlayerUnitMovementComponent>().With<UpdateField>().With<WeightComponent>();
        enemyEntities = this.World.Filter.With<EnemyUnitMovementComponent>().With<UpdateField>().With<WeightComponent>();
        playField = this.World.Filter.With<PlayField>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var field in playField)
        {
            ref var fieldComponent = ref field.GetComponent<PlayField>();

            foreach (var item in playerEntities)
            {
                CalculateWeight(item, ref fieldComponent);
                item.SetComponent(new UnitIsStay());
                item.RemoveComponent<UpdateField>();
            }
            foreach (var item in enemyEntities)
            {
                CalculateWeight(item, ref fieldComponent, true);
                item.SetComponent(new UnitIsStay());
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

    void CalculateWeight(Entity entity, ref PlayField fieldComponent, bool isEnemy = false)
    {
        ref WeightComponent weightComponent = ref entity.GetComponent<WeightComponent>();
        ref var update = ref entity.GetComponent<UpdateField>();
        if (update.UpdateWithReset)
        {
            ref var movementComponent = ref entity.GetComponent<PlayerUnitMovementComponent>();
            ResetWeight(ref weightComponent, ref fieldComponent);
            movementComponent.OccupiedNode = movementComponent.NextNode;
        }
        SetWeight(ref weightComponent, ref fieldComponent, isEnemy);
    }

    void ResetWeight(ref WeightComponent weightComponent, ref PlayField fieldComponent)
    {
        var center = GetCenter(weightComponent.Transform.position, ref fieldComponent);
        fieldComponent.Fields[center.x, center.y].isAvailable = true;

        foreach (var item in weightComponent.InfluenceArea.Keys)
        {
            fieldComponent.Fields[item.x, item.y].WeightForPlayer = weightComponent.InfluenceArea[item].x;
            fieldComponent.Fields[item.x, item.y].WeightForEnemy = weightComponent.InfluenceArea[item].y;
        }
        weightComponent.InfluenceArea.Clear();
    }

    void SetWeight(ref WeightComponent weightComponent, ref PlayField fieldComponent, bool isEnemy)
    {
        var center = GetCenter(weightComponent.Transform.position, ref fieldComponent);

        fieldComponent.Fields[center.x, center.y].isAvailable = false;

        weightComponent.InfluenceArea = SetWeight(center, weightComponent.IncreaseWeightAndOffset, ref fieldComponent, false,  isEnemy);
        SetWeight(center, weightComponent.DecreaseWeightAndOffset, ref fieldComponent, true, isEnemy);
    }

    Dictionary<Vector2Int, Vector2Int> SetWeight(Vector2Int center, Vector2Int weightAndOffset, ref PlayField fieldComponent, bool isDecrease, bool isEnemy)
    {
        Dictionary<Vector2Int, Vector2Int> result = new Dictionary<Vector2Int, Vector2Int>();
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

                result.Add(new Vector2Int(x, z), new Vector2Int(z, 0));

                if (isDecrease)
                {
                    node.WeightForEnemy += influence;
                    node.WeightForPlayer += influence;
                }
                else if (isEnemy)
                {
                    node.WeightForPlayer += influence;
                }
                else
                {
                    node.WeightForEnemy += influence;
                }
            }
        }
        return result;
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
}