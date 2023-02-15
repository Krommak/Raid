using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(InitFields))]
public sealed class InitFields : Initializer
{
    [SerializeField]
    float nodeRadius;
    [SerializeField]
    LayerMask grassMask;
    [SerializeField]
    Vector3 fieldSize;
    [SerializeField]
    Vector3 firstPoint = new Vector3(-3, 0.5f, 0);

    public override void OnAwake()
    {
        var field = this.World.CreateEntity();
        ref var component = ref field.AddComponent<PlayField>();

        int xSize = (int)(fieldSize.x / (nodeRadius));
        int zSize = (int)(fieldSize.z / (nodeRadius));
        component.Fields = new FieldNode[xSize, zSize];
        component.FirstPoint = firstPoint;
        component.NodeRadius = nodeRadius;
        var actualX = firstPoint.x + nodeRadius;
        var actualZ = firstPoint.z + nodeRadius;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var pos = new Vector3(actualX, firstPoint.y, actualZ);
                bool available = !Physics.CheckSphere(pos, nodeRadius, grassMask);
                ref var node = ref component.Fields[x, z];
                node.Position = pos;
                node.isAvailable = available;
                actualX += nodeRadius * 2;
            }
            actualX = firstPoint.x + nodeRadius;
            actualZ += nodeRadius * 2;
        }

        var draw = this.World.Filter.With<FiledDrawerComponent>();

        foreach (var item in draw)
        {
            ref var comp = ref item.GetComponent<FiledDrawerComponent>();

            comp.Drawer.DrawNodes(component.Fields);
        }
    }

    public override void Dispose()
    {

    }
}