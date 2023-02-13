using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlatformSystem))]
public sealed class PlatformSystem : UpdateSystem
{
    [SerializeField]
    LayerMask unitsLayer;
    Filter platforms;
    public override void OnAwake()
    {
        platforms = this.World.Filter.With<PlatformComponent>();
        foreach (var item in platforms)
        {
            ref var component = ref item.GetComponent<PlatformComponent>();
            component.TextMesh.text = component.UnitCount.ToString();
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in platforms)
        {
            ref var component = ref item.GetComponent<PlatformComponent>();

            var pos = component.PlatformTransform.position;
            Collider[] hitColliders = Physics.OverlapBox(pos, component.PlatformTransform.lossyScale, Quaternion.identity, unitsLayer);

            foreach (var unit in hitColliders)
            {
                PlatformCollision(ref component, unit, item);
            }
            if (component.UnitCount == 0)
                component.PlatformTransform.gameObject.SetActive(false);
        }
    }

    void PlatformCollision(ref PlatformComponent component, Collider hit, Entity entity)
    {
        switch (component.PlatformType)
        {
            case PlatformType.Plus:
                {
                    entity.SetComponent(new GetUnitComponent() 
                    { 
                        Position = component.PlatformTransform.position,
                        UnitsCount = component.UnitCount,
                        TimeForRespawn = 0.2f,
                    });
                    component.UnitCount = 0;
                    break;
                }
            case PlatformType.Danger:
                {
                    if (component.UnitCount >= 0) return;

                    var unit = hit.GetComponent<BattleUnitProvider>().Entity;
                    unit.SetComponent(new DeathComponent
                    {
                        DeathUnit = unit,
                        UnitGO = hit.gameObject
                    });
                    component.UnitCount++;
                    component.TextMesh.text = component.UnitCount.ToString();
                    break;
                }
        }
    }
}