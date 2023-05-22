using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnUI : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "Playground";
    [SerializeField] private string controlsScene = "ControlsUI";
    [SerializeField] private string menuScene = "MenuScene";
    public void NewGameButton()
    {
        SceneChanger(newGameLevel);
    }
    public void ControlsScreen()
    {
        SceneChanger(controlsScene);

    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    public void BackToMenu()
    {
        SceneChanger(menuScene);
    }

    public void SceneChanger(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
