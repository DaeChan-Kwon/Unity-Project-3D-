using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class pausemanager : MonoBehaviour
{
    public static bool isPaused = false;

    public TextMeshProUGUI pauseUI;
    public TextMeshProUGUI menuUI;

    public float loadTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused)
            {
                isPaused = false;
                Time.timeScale = 1f;
                pauseUI.text = "";
                menuUI.text = "";
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0f;
                pauseUI.text = "PAUSE";
                menuUI.text = "CONTINUE:ESC\nRESTART:SPACE";
            }
        }
        if (Input.GetButtonDown("Jump") && isPaused)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("ShootingGame");
        }
    }
}
