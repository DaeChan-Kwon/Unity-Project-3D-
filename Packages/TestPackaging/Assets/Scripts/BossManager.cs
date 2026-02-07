using UnityEngine;

public class BossManager : MonoBehaviour
{
    float currentTime = 0;

    // 인스펙터에서 설정 가능한 보스 생성 간격 (초)
    public float createTime = 30f;
    public GameObject enemy; // Boss 프리팹 연결

    void Start()
    {
        // 초기화 필요 없음
    }

    void Update()
    {
        // 경과 시간 누적
        currentTime += Time.deltaTime;

        // 설정된 시간(createTime) 경과 시 보스 생성
        if (currentTime > createTime)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);

            // 타이머 초기화로 다음 보스 생성 시간 카운트 시작
            currentTime = 0;
        }

        // 랜덤 범위 코드 제거 필요 (중복 생성 방지)
    }
}
