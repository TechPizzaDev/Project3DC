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
}
