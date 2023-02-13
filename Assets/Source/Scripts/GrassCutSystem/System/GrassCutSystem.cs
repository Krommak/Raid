using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(GrassCutSystem))]
public sealed class GrassCutSystem : FixedUpdateSystem
{
    [SerializeField]
    private float cutSpeed;
    [SerializeField]
    private float sizeYForDisable;
    Filter grassCuts;

    public override void OnAwake()
    {
        grassCuts = this.World.Filter.With<GrassCutComponent>().Without<GrassCutDone>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in grassCuts)
        {
            var transform = item.GetComponent<GrassCutComponent>().Transform;
            var localScale = transform.localScale;
            transform.localScale = new Vector3(localScale.x, localScale.y * cutSpeed, localScale.z);
            if (transform.localScale.y < sizeYForDisable)
            {
                item.AddComponent<GrassCutDone>();
                transform.gameObject.SetActive(false);
            }
        }
        var taps = this.World.Filter.With<InputPressedTag>().With<PlayerInputComponent>();
    }
}