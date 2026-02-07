using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "GameScene"; // 시작할 게임 씬 이름

    // 게임 시작 버튼
    public void StartGame()
    {
        Time.timeScale = 1f; // 게임이 멈춰있을 때 대비
        Debug.Log("게임 시작: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName); // 씬 전환
    }

    // 게임 종료 버튼
    public void QuitGame()
    {
        Debug.Log("게임 종료");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서는 실행 종료
#else
        Application.Quit(); // 빌드된 게임에서는 종료
#endif
    }
}
