using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(FingerCollisionSystem))]
public sealed class FingerCollisionSystem : UpdateSystem
{
    [SerializeField]
    LayerMask LayerMask;
    [SerializeField]
    float ovelapSize;

    Filter taps;
    public override void OnAwake()
    {
        taps = this.World.Filter.With<InputPressedTag>().With<PlayerInputComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in taps)
        {
            ref var tap = ref item.GetComponent<PlayerInputComponent>();

            var position = Vector3.Lerp(tap.TapPosition, tap.LastOverlapPosition, 0.1f);
            tap.LastOverlapPosition = position;

            Collider[] hitColliders = Physics.OverlapSphere(position, ovelapSize, LayerMask);
            foreach (var collider in hitColliders)
            {
                var entity = this.World.CreateEntity();
                entity.SetComponent(new GrassCutComponent
                {
                    Transform = collider.transform
                });
                collider.enabled = false;
            }
        }
    }
}