using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public SceneAsset MainMenuScene;
    public SceneAsset ControlsMenuScene;

    public SceneAsset GameplayScene;
    public SceneAsset ShopMenuScene;

    public SceneAsset WinScene;
    public SceneAsset GameOverScene;

    public void GoToMainMenuScene()
    {
        SceneManager.LoadSceneAsync(MainMenuScene.name, LoadSceneMode.Single);
    }

    public void OpenControlsMenuScene()
    {
        SceneManager.LoadSceneAsync(ControlsMenuScene.name, LoadSceneMode.Additive);
    }

    public void GoToGameplayScene()
    {
        SceneManager.LoadSceneAsync(GameplayScene.name, LoadSceneMode.Single);
    }

    public void OpenShopMenuScene()
    {
        SceneManager.LoadSceneAsync(ShopMenuScene.name, LoadSceneMode.Single);
    }

    public void GoToWinScene()
    {
        SceneManager.LoadSceneAsync(WinScene.name, LoadSceneMode.Single);
    }

    public void GoToGameOverScene()
    {
        SceneManager.LoadSceneAsync(GameOverScene.name, LoadSceneMode.Single);
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
