using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// The screen manager provides functions for transitions between scenes.
/// </summary>
/// <remarks>
/// Since scene transitions are destructive, 
/// we also need <see cref="LevelStateInstance"/> to keep track of state between scenes.
/// </remarks>
public class ScreenManager : MonoBehaviour
{
    // TODO: look into cleaning up/handling LoadSceneAsync.completed events in an organized matter

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

    // FIXME: The current code that manages actually saves/restores state is dirty and all over the place
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

    // TODO: Unity Input system is behaving strangely between scenes,
    // and scenes that don't have an input manager can't unlock their cursor,
    // but the input manager could not exist without a player,
    // and a player instance behind e.g. the shop caused problems, 
    // so this is the current workaround.
    void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToMainMenuScene()
    {
        SceneManager.LoadSceneAsync(MainMenuScene.BuildIndex, LoadSceneMode.Single).completed += (ev) => UnlockCursor();
    }

    public void OpenControlsMenuScene()
    {
        SceneManager.LoadSceneAsync(ControlsMenuScene.BuildIndex, LoadSceneMode.Additive);
    }

    public void GoToGameplayScene()
    {
        if (Player != null)
        {
            var levelState = LevelState.Instance;
            levelState.PlayerHealth = Player.health;
            levelState.PlayerMaxHealth = Player.maxHealth;
            levelState.PlayerCurrency = Player.currency;
        }

        var op = SceneManager.LoadSceneAsync(GameplayScene.BuildIndex, LoadSceneMode.Single);
        op.completed += ScreenManager_GameplayLoadCompleted;
    }

    private void AssignPlayerValues()
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

    private void ScreenManager_GameplayLoadCompleted(AsyncOperation obj)
    {
        AssignPlayerValues();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToShopMenuScene()
    {
        var levelState = LevelState.Instance;
        levelState.PlayerHealth = Player.health;
        levelState.PlayerMaxHealth = Player.maxHealth;
        levelState.PlayerCurrency = Player.currency;

        SceneManager.LoadSceneAsync(ShopMenuScene.BuildIndex, LoadSceneMode.Single).completed += ScreenManager_completed;
    }

    private void ScreenManager_completed(AsyncOperation obj)
    {
        AssignPlayerValues();
        UnlockCursor();
    }

    public void GoToWinScene()
    {
        SceneManager.LoadSceneAsync(WinScene.BuildIndex, LoadSceneMode.Single).completed += (ev) => UnlockCursor(); ;
    }

    public void GoToGameOverScene()
    {
        SceneManager.LoadSceneAsync(GameOverScene.BuildIndex, LoadSceneMode.Single).completed += (ev) => UnlockCursor(); ;
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
