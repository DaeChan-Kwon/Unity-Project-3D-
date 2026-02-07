using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 스폰 설정
    public GameObject enemyPrefab;
    public Transform player;

    // 스폰 범위
    public Vector2 areaSize = new Vector2(60f, 60f);
    public float spawnHeight = 10f;

    // 스폰 제어
    public float spawnInterval = 0.05f;
    public int maxActiveEnemies = 50;

    float timer = 0f;
    int currentActiveEnemies = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 스폰 주기 확인 및 최대 개체 수 제한
        if (timer >= spawnInterval && currentActiveEnemies < maxActiveEnemies)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    // 적 1개 생성
    void SpawnOne()
    {
        if (enemyPrefab == null || player == null) return;

        // 랜덤 스폰 위치 계산
        Vector3 basePos = transform.position;
        float halfX = areaSize.x * 0.5f;
        float halfZ = areaSize.y * 0.5f;
        float randX = Random.Range(-halfX, halfX);
        float randZ = Random.Range(-halfZ, halfZ);

        Vector3 spawnPos = new Vector3(
            basePos.x + randX,
            player.position.y + spawnHeight,
            basePos.z + randZ
        );

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentActiveEnemies++;
    }

    // 적 사망 시 호출 (EnemyFSM에서 호출)
    public void OnEnemyDead()
    {
        currentActiveEnemies = Mathf.Max(0, currentActiveEnemies - 1);
    }

    // 스폰 범위 시각화 (에디터)
    void OnDrawGizmosSelected()
    {
        // 지상 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 0.1f, areaSize.y));

        // 스폰 높이
        Vector3 center = transform.position;
        center.y = player != null ? player.position.y + spawnHeight : center.y + spawnHeight;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, 0.1f, areaSize.y));
    }
}