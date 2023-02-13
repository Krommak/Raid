using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PlayerInputSystem))]
public sealed class PlayerInputSystem : UpdateSystem
{
    Entity tap;
    Filter cameraFilter;

    public override void OnAwake()
    {
        tap = this.World.CreateEntity();
        tap.AddComponent<PlayerInputComponent>();
        cameraFilter = this.World.Filter.With<PlayerCameraComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (Input.GetMouseButtonDown(0))
        {
            tap.AddComponent<InputPressedTag>();

            ref var inputComponent = ref tap.GetComponent<PlayerInputComponent>();

            foreach (var item in cameraFilter)
            {
                ref var cameraComponent = ref item.GetComponent<PlayerCameraComponent>();
                Ray ray = cameraComponent.Camera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f, cameraComponent.RaycastLayer);
                inputComponent.TapPosition = hitInfo.point;
                inputComponent.LastOverlapPosition = hitInfo.point;
            }
        }
        if (Input.GetMouseButton(0))
        {
            ref var inputComponent = ref tap.GetComponent<PlayerInputComponent>();

            foreach (var item in cameraFilter)
            {
                ref var cameraComponent = ref item.GetComponent<PlayerCameraComponent>();
                Ray ray = cameraComponent.Camera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f, cameraComponent.RaycastLayer);
                inputComponent.TapPosition = hitInfo.point;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            tap.RemoveComponent<InputPressedTag>();
        }
    }
}