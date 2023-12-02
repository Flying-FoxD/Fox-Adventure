using UnityEngine;
using UnityEngine.UI;

public class HiScoreText : MonoBehaviour
{
    [SerializeField] private int LevelNummer;
    [SerializeField] private Text hiScore;
    private void Awake()
    {
        hiScore.text = PlayerPrefs.GetFloat("Level " +  LevelNummer).ToString("0.00") + "sec"; 
    }
}
