using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "OverWorld";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
