using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.Collections.Generic;
using System.Reflection;

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
    Entity finger;

    public override void OnAwake()
    {
        taps = this.World.Filter.With<InputPressedTag>().With<PlayerInputComponent>();
        var entities = this.World.Filter.With<FingerComponent>();
        foreach (var item in entities)
        {
            finger = item;
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in taps)
        {
            ref var tap = ref item.GetComponent<PlayerInputComponent>();

            var position = Vector3.Lerp(tap.TapPosition, tap.LastOverlapPosition, 0.1f);
            tap.LastOverlapPosition = position;

            ref var component = ref finger.GetComponent<FingerComponent>();
            component.Transform.position = position;

            Collider[] hitColliders = Physics.OverlapSphere(position, ovelapSize, LayerMask);
            foreach (var collider in hitColliders)
            {
                collider.enabled = false;
            }
        }
    }
}