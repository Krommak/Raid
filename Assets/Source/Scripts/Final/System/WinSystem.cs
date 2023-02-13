using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(WinSystem))]
public sealed class WinSystem : UpdateSystem
{
    [SerializeField]
    LayerMask unitsLayer;
    Filter finish;
    public override void OnAwake()
    {
        finish = this.World.Filter.With<FinishComponent>().Without<WinComponent>().Without<FallComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in finish)
        {
            ref var component = ref item.GetComponent<FinishComponent>();

            Collider[] hits = Physics.OverlapBox(component.Transform.position, component.Transform.localScale/2, Quaternion.identity, unitsLayer);

            if (hits.Length > 0)
            {
                item.SetComponent(new WinComponent());
                component.Particle.Play();
            }
        }
    }
}