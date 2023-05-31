using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    public GameObject LevelStatePrefab;

    public SceneReference MainMenuScene;
    public SceneReference ControlsMenuScene;

    public SceneReference GameplayScene;
    public SceneReference ShopMenuScene;

    public SceneReference WinScene;
    public SceneReference GameOverScene;

    public static GameObject LevelStateInstance { get; private set; }

    private UnitHealth Player;

    void Awake()
    {
        Instance = this;

        if (LevelStateInstance == null)
        {
            LevelStateInstance = Instantiate(LevelStatePrefab);
            DontDestroyOnLoad(LevelStateInstance);
        }

        RefreshPlayer();
    }

    void RefreshPlayer()
    {
        Player = null;
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.GetComponent<UnitHealth>();
        }
    }

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
        var op = SceneManager.LoadSceneAsync(GameplayScene.BuildIndex, LoadSceneMode.Single);
        op.completed += ScreenManager_GameplayLoadCompleted;
    }

    private void ScreenManager_GameplayLoadCompleted(AsyncOperation obj)
    {
        RefreshPlayer();

        if (Player != null)
        {
            var levelState = LevelState.Instance;
            Player.health = levelState.PlayerHealth;
            Player.maxHealth = levelState.PlayerMaxHealth;
            Player.currency = levelState.PlayerCurrency;
        }
    }

    public void GoToShopMenuScene()
    {
        var levelState = LevelState.Instance;
        levelState.PlayerHealth = Player.health;
        levelState.PlayerMaxHealth = Player.maxHealth;
        levelState.PlayerCurrency = Player.currency;

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
