using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 관리를 위한 네임스페이스

public class Menu : MonoBehaviour
{
    // "게임 시작" 버튼 클릭 시 호출되는 함수
    public void StartGame()
    {
        // 메인게임 씬을 불러옴
        SceneManager.LoadScene("2271031권대찬");
    }

    // "게임 종료" 버튼 클릭 시 호출되는 함수
    public void QuitGame()
    {
        // 유니티 에디터에서는 종료가 되지 않고 로그만 출력됨
        Debug.Log("게임 종료 버튼 클릭!");
        // 실제 빌드된 실행 파일(exe)에서는 게임 실행을 종료함
        Application.Quit();
    }
}
