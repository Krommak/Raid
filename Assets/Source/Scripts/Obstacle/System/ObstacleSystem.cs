using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ObstacleSystem))]
public sealed class ObstacleSystem : UpdateSystem
{
    [SerializeField]
    LayerMask unitsLayer;
    Filter obstacles;
    public override void OnAwake()
    {
        obstacles = this.World.Filter.With<ObstacleComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in obstacles)
        {
            ref var component = ref item.GetComponent<ObstacleComponent>();

            Collider[] hitColliders = Physics.OverlapBox(component.ObstTransform.position, component.BoxSize, Quaternion.identity, unitsLayer);
            foreach (var hit in hitColliders)
            {
                var unit = hit.GetComponent<BattleUnitProvider>().Entity;
                unit.SetComponent(new ActiveDamageComponent());
                unit.SetComponent(new DeathComponent
                {
                    DeathUnit = unit,
                    UnitGO = hit.gameObject
                });
            }
        }
    }
}