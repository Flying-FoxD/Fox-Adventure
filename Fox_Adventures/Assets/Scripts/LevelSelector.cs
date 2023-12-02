using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private Text levelText;
    // Start is called before the first frame update
    void Start()
    {
        levelText.text = levelNumber.ToString();
    }

    public void home()
    {
        SceneManager.LoadScene("Home");
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene("Level " + levelNumber);
    }
}
