using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    float currentTime = 0;
    public float createTime = 5;
    public GameObject enemy; // 'enemy' 프리팹

    // --- 기본 스폰 시간 ---
    public float minTime = 1f;
    public float maxTime = 5f;

    // --- 점수 1500점 넘었을 때 강화 스폰 시간 변수 ---
    public float buffedMinTime = 0.5f;  // 강화 후 최소 스폰 시간
    public float buffedMaxTime = 2.0f;  // 강화 후 최대 스폰 시간
    public int difficultyScoreThreshold = 1500; // 난이도 상승 기준 점수

    private Camera mainCamera;
    private ScoreManager scoreManager;  // 점수 관리자 참조

    void Start()
    {
        mainCamera = Camera.main;

        // 씬에서 ScoreManager 찾기
        GameObject smObject = GameObject.Find("ScoreManager");
        if (smObject != null)
        {
            scoreManager = smObject.GetComponent<ScoreManager>();
        }
        else
        {
            Debug.LogError("EnemyManager: 'ScoreManager'를 찾을 수 없습니다!");
        }

        // 첫 적 생성 시간 랜덤 지정
        createTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        if (scoreManager == null) return; // 점수 관리자가 없으면 실행하지 않음

        currentTime += Time.deltaTime;

        if (currentTime > createTime)
        {
            // 뷰포트 기준 적 스폰 위치 (랜덤 X, 화면 위쪽 Y)
            float randomX = Random.Range(0.1f, 0.9f);
            float topY = 1.1f;
            Vector3 randomViewportPos = new Vector3(randomX, topY, Mathf.Abs(mainCamera.transform.position.z));
            Vector3 spawnPos = mainCamera.ViewportToWorldPoint(randomViewportPos);
            spawnPos.z = 0;

            // 적 생성
            if (enemy != null)
            {
                Instantiate(enemy, spawnPos, Quaternion.identity);
            }

            // 타이머 초기화
            currentTime = 0;

            // 점수에 따라 다음 적 스폰 간격을 짧게 또는 기본으로 설정
            if (scoreManager.currentScore > difficultyScoreThreshold)
            {
                createTime = Random.Range(buffedMinTime, buffedMaxTime);
            }
            else
            {
                createTime = Random.Range(minTime, maxTime);
            }
        }
    }
}
