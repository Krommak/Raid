using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using CodeWriter.MeshAnimation;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct UnitAnimatorComponent : IComponent
{
    [SerializeField]
    public MeshAnimator Animator;
    internal UnitAnimationState CurrentState;
}
