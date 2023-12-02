using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
public void Home()
    {
        SceneManager.LoadScene("Home");
    }
    public void resetHiScore()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("LevelSelection");
    }
}
