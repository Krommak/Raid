using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ScoreSystem))]
public sealed class ScoreSystem : UpdateSystem
{
    Filter scoreCounters;
    Filter increases;
    Filter decreases;
    public override void OnAwake()
    {
        scoreCounters = this.World.Filter.With<CountComponent>();

        foreach (var item in scoreCounters)
        {
            ref var component = ref item.GetComponent<CountComponent>();

            component.ScoreText.text = 0.ToString();
            component.Score = 0;
        }
        increases = this.World.Filter.With<IncreaseScoreComponent>();
        decreases = this.World.Filter.With<DecreaseScoreComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var item in scoreCounters)
        {
            ref var component = ref item.GetComponent<CountComponent>();
            component.Score += (increases.GetLengthSlow() - decreases.GetLengthSlow());
            component.ScoreText.text = component.Score.ToString();

            foreach (var inc in increases)
            {
                inc.RemoveComponent<IncreaseScoreComponent>();
            }
            foreach (var dec in decreases)
            {
                if (component.Score == 0)
                {
                    var finish = this.World.Filter.With<FinishComponent>();
                    foreach (var element in finish)
                    {
                        element.SetComponent(new FallComponent());
                    }
                }
                dec.RemoveComponent<DecreaseScoreComponent>();
            }

        }
    }
}