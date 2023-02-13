using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using UnityEngine.SceneManagement;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(InitButtons))]
public sealed class InitButtons : Initializer
{
    public override void OnAwake()
    {
        var buttons = this.World.Filter.With<ButtonsComponent>();

        foreach (var item in buttons)
        {
            ref var component = ref item.GetComponent<ButtonsComponent>();

            component.QuitButton.onClick.AddListener(QuitGame);
            component.RestartButton.onClick.AddListener(RestartScene);
            component.RestartButton.gameObject.SetActive(false);
        }
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public override void Dispose()
    {
        var buttons = this.World.Filter.With<ButtonsComponent>();

        foreach (var item in buttons)
        {
            ref var component = ref item.GetComponent<ButtonsComponent>();

            component.QuitButton.onClick.RemoveListener(QuitGame);
            component.RestartButton.onClick.RemoveListener(RestartScene);
        }
    }
}