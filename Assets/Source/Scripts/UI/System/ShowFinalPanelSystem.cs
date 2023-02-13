using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ShowFinalPanelSystem))]
public sealed class ShowFinalPanelSystem : UpdateSystem
{
    Filter winAndFall;
    Filter showed;
    Filter fall;
    Filter win;
    Filter buttons;
    public override void OnAwake()
    {
        winAndFall = this.World.Filter.With<WinAndFallComponent>();
        showed = this.World.Filter.With<FinalIsShowed>();
        fall = this.World.Filter.With<FallComponent>();
        win = this.World.Filter.With<WinComponent>();
        buttons = this.World.Filter.With<ButtonsComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (showed.GetLengthSlow() > 0) return;

        foreach (var item in fall)
        {
            foreach (var element in winAndFall)
            {
                element.GetComponent<WinAndFallComponent>().Fall.SetActive(true);
                element.SetComponent(new FinalIsShowed());
                ShowRestartButton();
            }
        }
        foreach (var item in win)
        {
            foreach (var element in winAndFall)
            {
                element.GetComponent<WinAndFallComponent>().Win.SetActive(true);
                element.SetComponent(new FinalIsShowed());
                ShowRestartButton();
            }
        }
    }

    void ShowRestartButton()
    {
        foreach (var item in buttons)
        {
            ref var component = ref item.GetComponent<ButtonsComponent>();
;
            component.RestartButton.gameObject.SetActive(true);
        }
    }
}