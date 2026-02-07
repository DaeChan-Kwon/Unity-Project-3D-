using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    // 현재 점수를 표시하는 UI 텍스트 컴포넌트
    public TextMeshProUGUI currentUI;
    // 현재 점수 값
    public int currentScore = 0;
    // 최고 점수를 표시하는 UI 텍스트 컴포넌트
    public TextMeshProUGUI bestUI;
    // 최고 점수 값
    public int bestScore = 0;

    // 게임 오버 상태를 저장하는 변수
    private bool isGameOver = false;

    void Start()
    {
        // 저장된 최고 점수를 불러오고 UI에 표시
        bestScore = PlayerPrefs.GetInt("Best Score", 0);
        bestUI.text = "Best Score: " + bestScore;
    }

    void Update()
    {
        // 게임이 끝난 상태이고 스페이스바를 누르면
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            // 게임 일시 정지 상태를 해제하여 시간 흐름을 정상화하고
            Time.timeScale = 1f;
            // 현재 씬을 다시 로드하여 게임을 재시작
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // 플레이어가 게임 오버 시 호출하는 함수로 게임 오버 상태를 true로 변경
    public void TriggerGameOver()
    {
        isGameOver = true;
    }
}
