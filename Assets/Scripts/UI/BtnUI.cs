using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnUI : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "Playground";
    public void NewGameButton()
    {
        SceneManager.LoadScene(newGameLevel);
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
