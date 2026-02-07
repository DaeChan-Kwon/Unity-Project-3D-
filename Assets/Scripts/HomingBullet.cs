using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 20f;              // 총알 이동 속도
    public float rotationSpeed = 10f;      // 회전 속도 (유도 회전)

    public Transform target;               // 유도 목표물(가장 가까운 적)

    void Start()
    {
        // 발사 시 가장 가까운 적을 목표로 설정
        target = FindClosestEnemy();
    }

    void Update()
    {
        // 목표가 없으면 다시 찾아보고 없으면 직진
        if (target == null)
        {
            target = FindClosestEnemy();
            if (target == null)
            {
                // 목표가 없으면 직선으로 이동
                transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
                return;
            }
        }

        // 목표 방향 계산 (정규화된 벡터)
        Vector3 direction = (target.position - transform.position).normalized;

        // 목표 방향으로의 회전(2D용 LookRotation)
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        // 현재 회전에서 목표 회전으로 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 자기 방향(앞쪽)으로 이동
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    // 가장 가까운 적을 찾아 반환하는 함수
    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }
}
