using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MoveToEnemySystem))]
public sealed class MoveToEnemySystem : UpdateSystem
{
    [SerializeField]
    float unitSpeed;
    Filter moves;
    public override void OnAwake()
    {
        moves = this.World.Filter.With<MoveToEnemyComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in moves)
        {
            ref var component = ref item.GetComponent<MoveToEnemyComponent>();

            var direction = component.TargetPosition - component.UnitRB.position;
            component.UnitRB.velocity = direction.normalized * unitSpeed;

            item.RemoveComponent<MoveToEnemyComponent>();
        }
    }
}