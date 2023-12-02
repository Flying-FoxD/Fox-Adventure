using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool playerAlive = true;
    public float scoreCount;
    private float HiScore;
    private bool scoreTimer;
    private string activeLevel;
    private bool gamePause = false;
    [SerializeField] private int restartDelay = 1;
    [SerializeField] private Text playerScore;
    [SerializeField] private Text hiScoreText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private GameObject escMenuScreen;
    [SerializeField] private GameObject newHiScoreScreen;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
    }
    void Start()
    {
        Life(true);
        activeLevel = SceneManager.GetActiveScene().name;
        
        hiScoreText.text = ("Hi-Score: " +PlayerPrefs.GetFloat(activeLevel).ToString("0.00"));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            escMenu();
        }
        if (scoreTimer == true)
        {
            scoreCount += 1f * Time.deltaTime;
            playerScore.text = ("Score: " + scoreCount.ToString("0.00"));
            
        }
    }
    private void Life(bool life)
    {
        if (life)
        {
            scoreTimer = true;
            playerAlive = true;
            scoreCount = 0;
        }
        else
        {
            scoreTimer = false;
            playerAlive = false;
        }
    }
    public void RestartLevel()
    {
        Life(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GameOver()
    {
        Life(false);
        gameOverScreen.SetActive(true);
            Invoke("RestartLevel", restartDelay);
    }
    public void LevelCompleted()
    {
        Life(false);
        HiScore = PlayerPrefs.GetFloat(activeLevel);
        if (HiScore > scoreCount || HiScore == 0) 
        {
            newHiScoreScreen.SetActive(true);
            PlayerPrefs.SetFloat(activeLevel, scoreCount);
            hiScoreText.text = ("Hi-Score: " + PlayerPrefs.GetFloat(activeLevel).ToString("0.00"));
        }
        else if (HiScore < scoreCount)
        {
            finishScreen.SetActive(true);
        }
    }
    public void nextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < (int)SceneManager.sceneCountInBuildSettings -1f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Life(true);
        }
        else
        {
            homeScreen();
        }
    }
    public void homeScreen()
    {
        SceneManager.LoadScene(0);
    }
    public void levelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }
    private void escMenu()
    {
        if (gamePause == false)
        {
            playerAlive = false;
            scoreTimer = false;
            gamePause = true;
            escMenuScreen.SetActive(true);
        }
        else if (gamePause)
        {
            scoreTimer = true;
            gamePause = false;
            escMenuScreen.SetActive(false);
        }

    }
}



