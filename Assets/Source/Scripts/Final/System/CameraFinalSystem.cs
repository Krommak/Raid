using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using UnityEngine.UIElements;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CameraFinalSystem))]
public sealed class CameraFinalSystem : LateUpdateSystem
{
    [SerializeField]
    float FinalCameraSpeed;
    Filter final;
    Filter cameras;
    public override void OnAwake()
    {
        final = this.World.Filter.With<FinishComponent>().With<WinComponent>();
        cameras = this.World.Filter.With<CameraMovingComponent>().Without<CameraOnFinalPosition>();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (final.GetLengthSlow() == 0) return;

        foreach (var item in cameras)
        {
            ref var moving = ref item.GetComponent<CameraMovingComponent>();
            var transform = moving.CameraTransform;
            var newPosition = Vector3.Lerp(transform.position, moving.finalPosition, FinalCameraSpeed * deltaTime);
            transform.position = newPosition;
            var newRotation = Vector3.Lerp(transform.eulerAngles, moving.finalRotation, FinalCameraSpeed * deltaTime);
            transform.eulerAngles = newRotation;
            if((newPosition - moving.finalPosition).magnitude < 0.1f && Vector3.Angle(newRotation, moving.finalRotation) < 0.1f)
            {
                item.SetComponent(new CameraOnFinalPosition());
            }
        }
    }
}