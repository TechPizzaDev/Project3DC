using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScreenManager : MonoBehaviour
{
    private static GameObject musicInstance;

    public static ScreenManager Instance { get; private set; }

    public GameObject LevelStatePrefab;
    public GameObject MusicPrefab;

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

        if (musicInstance == null)
        {
            musicInstance = Instantiate(MusicPrefab);
            DontDestroyOnLoad(musicInstance);
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

    void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToMainMenuScene()
    {
        UnlockCursor();
        SceneManager.LoadSceneAsync(MainMenuScene.BuildIndex, LoadSceneMode.Single);
    }

    public void OpenControlsMenuScene()
    {
        UnlockCursor();
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
        UnlockCursor();
        SceneManager.LoadSceneAsync(WinScene.BuildIndex, LoadSceneMode.Single);
    }

    public void GoToGameOverScene()
    {
        UnlockCursor();
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
