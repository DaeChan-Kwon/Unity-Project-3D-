using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class scoremanager : MonoBehaviour
{
    public TextMeshProUGUI currentUI;
    public int currentScore = 0;

    public TextMeshProUGUI bestUI;
    public int bestScore = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bestScore = PlayerPrefs.GetInt("Best Score", 0);
        bestUI.text = "Best Score: " + bestScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
