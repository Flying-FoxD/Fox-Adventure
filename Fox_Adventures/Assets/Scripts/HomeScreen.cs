using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreen : MonoBehaviour
{
    public void quit()
    {
        Application.Quit();
    }
    public void Level()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}
