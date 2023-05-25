using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScreenManager : MonoBehaviour
{
    public SceneReference MainMenuScene;
    public SceneReference ControlsMenuScene;

    public SceneReference GameplayScene;
    public SceneReference ShopMenuScene;
                
    public SceneReference WinScene;
    public SceneReference GameOverScene;

    public void GoToMainMenuScene()
    {
        SceneManager.LoadSceneAsync(MainMenuScene.BuildIndex, LoadSceneMode.Single);
    }

    public void OpenControlsMenuScene()
    {
        SceneManager.LoadSceneAsync(ControlsMenuScene.BuildIndex, LoadSceneMode.Additive);
    }

    public void GoToGameplayScene()
    {
        SceneManager.LoadSceneAsync(GameplayScene.BuildIndex, LoadSceneMode.Single);
    }

    public void OpenShopMenuScene()
    {
        SceneManager.LoadSceneAsync(ShopMenuScene.BuildIndex, LoadSceneMode.Single);
    }

    public void GoToWinScene()
    {
        SceneManager.LoadSceneAsync(WinScene.BuildIndex, LoadSceneMode.Single);
    }

    public void GoToGameOverScene()
    {
        SceneManager.LoadSceneAsync(GameOverScene.BuildIndex, LoadSceneMode.Single);
    }

    public void RestartGameplay()
    {
        GoToGameplayScene();
    }

    public void ContinueGameplayFromShopMenu()
    {
        GoToGameplayScene();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
