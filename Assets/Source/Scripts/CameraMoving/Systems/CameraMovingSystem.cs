using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using System.ComponentModel;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using TMPro;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CameraMovingSystem))]
public sealed class CameraMovingSystem : LateUpdateSystem
{
    [SerializeField]
    float timeForRecalculateTarget;
    float timeFromlastRecalculation;
    Filter nonFinal;
    Filter cameras;
    Filter targets;
    Filter activeBattles;
    Transform targetUnit;
    public override void OnAwake()
    {
        nonFinal = this.World.Filter.With<FinishComponent>().Without<WinComponent>().Without<FallComponent>();
        cameras = this.World.Filter.With<CameraMovingComponent>();
        targets = this.World.Filter.With<CameraTargetComponent>();
        activeBattles = this.World.Filter.With<BattleZoneIsActive>();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (nonFinal.GetLengthSlow() == 0) return;
        
        foreach (var cam in cameras)
        {
            if (timeFromlastRecalculation > timeForRecalculateTarget || targetUnit == null)
                RecalculateTargetUnit();

            timeFromlastRecalculation += deltaTime;

            if (targetUnit == null) return;

            var camera = cam.GetComponent<CameraMovingComponent>();

            var speed = activeBattles.GetLengthSlow() == 0 ? camera.CameraSpeed : camera.CameraSpeed / 10;
            var targetPosition = new Vector3(camera.CameraTransform.position.x, camera.CameraTransform.position.y, targetUnit.position.z + camera.DistanceAtFirstUnit);
            camera.CameraTransform.position = Vector3.Lerp(camera.CameraTransform.position, targetPosition, speed);
        }
    }

    void RecalculateTargetUnit()
    {
        timeFromlastRecalculation = 0;
        float targetPos = 0;
        foreach (var target in targets)
        {
            ref var component = ref target.GetComponent<CameraTargetComponent>();
            if (component.Transform.position.z > targetPos)
            {
                targetPos = component.Transform.position.z;
                targetUnit = component.Transform;
            }
        }
    }
}