using UnityEngine;

public class SpecialBullet : MonoBehaviour
{
    public float speed = 20f;       // 필살기 총알 속도
    public int bossDamage = 50;     // 보스에게 줄 데미지

    void Update()
    {
        // 총알이 위쪽 방향으로 지속 이동
        transform.position += Vector3.up * speed * Time.deltaTime;

        // 화면 밖(예: y 위치 30 이상)으로 나가면 자동 파괴
        if (transform.position.y > 30f)
        {
            Destroy(gameObject);
        }
    }

    // 관통탄으로서의 충돌 처리(3D 물리, Is Trigger 체크 필수)
    private void OnTriggerEnter(Collider other)
    {
        // 1. 일반 적과 충돌
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(9999);  // 원킬 데미지 처리
            }
            // 관통탄이므로 자기 파괴 없음
        }
        // 2. 보스와 충돌
        else if (other.CompareTag("Boss"))
        {
            Boss boss = other.GetComponentInParent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(bossDamage); // 보스에 지정 데미지 적용
            }
            // 보스도 관통 가능 (필요 시 Destroy(gameObject) 추가 가능)
        }
        // 3. 적 총알과 충돌
        else if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject); // 적 총알만 제거
        }
    }
}
