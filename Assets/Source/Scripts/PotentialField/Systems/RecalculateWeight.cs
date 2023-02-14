using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(RecalculateWeight))]
public sealed class RecalculateWeight : LateUpdateSystem
{
    Filter finishEntity;
    Filter enemyEntity;
    Filter playField;
    public override void OnAwake()
    {
        finishEntity = this.World.Filter.With<FinishWeightComponent>();
        enemyEntity = this.World.Filter.With<EnemyWeightComponent>();
        playField = this.World.Filter.With<PlayField>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var field in playField)
        {
            ref var fieldComponent = ref field.GetComponent<PlayField>();

            var xSize = fieldComponent.Fields.GetLength(0);
            var zSize = fieldComponent.Fields.GetLength(1);

            foreach (var item in enemyEntity)
            {
                ref var weightComponent = ref item.GetComponent<EnemyWeightComponent>();

                //var firstX = weightComponent.Transform.position.x - weightComponent.Offset;
                //var lastX = weightComponent.Transform.position.x + weightComponent.Offset;
                //var firstZ = weightComponent.Transform.position.z - weightComponent.Offset;
                //var lastZ = weightComponent.Transform.position.z + weightComponent.Offset;
                //var actualX = firstX;
                //var actualZ = firstZ;

            }
        }
    }
}