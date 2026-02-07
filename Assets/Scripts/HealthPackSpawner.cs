using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    public GameObject healthPackPrefab;
    public float minTime = 5f;
    public float maxTime = 10f;

    private float currentTime = 0f;
    private float createTime;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        createTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > createTime)
        {
            // 화면 안쪽 X좌표 랜덤 (0~1 범위)
            float randomX = Random.Range(0.1f, 0.9f);
            float topY = 1.1f; // 화면 위쪽 조금 위

            Vector3 viewportPos = new Vector3(randomX, topY, Mathf.Abs(mainCamera.transform.position.z));
            Vector3 spawnPos = mainCamera.ViewportToWorldPoint(viewportPos);
            spawnPos.z = 0f; // 2D용

            // 힐팩 생성
            Instantiate(healthPackPrefab, spawnPos, Quaternion.identity);

            // 다음 생성 시간 랜덤
            currentTime = 0f;
            createTime = Random.Range(minTime, maxTime);
        }
    }
}