using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenCloser : MonoBehaviour
{
    public void CloseScreen()
    {
        Scene scene = gameObject.scene;
        SceneManager.UnloadSceneAsync(scene);
    }
}
