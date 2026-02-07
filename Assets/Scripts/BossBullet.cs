using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1; // 이 총알이 플레이어에게 주는 데미지

    // ▼▼▼ (추가) 보스 총알 체력 ▼▼▼
    public int health = 3; // 예: 3대 맞으면 터짐
    // ▲▲▲ (추가) 보스 총알 체력 ▲▲▲

    void Update()
    {
        // 2D 기준: 자신의 '위쪽' (초록색 화살표) 방향으로 이동
        transform.position += transform.up * speed * Time.deltaTime;
    }

    // 플레이어 총알과 충돌했을 때
    // (이 스크립트를 쓰는 오브젝트와 PlayerBullet 양쪽 모두 Collider와 Rigidbody가 필요합니다)
    private void OnCollisionEnter(Collision collision)
    {
        // (주의) OnCollisionEnter를 사용하려면 Is Trigger를 체크 해제해야 합니다.
        // Is Trigger를 썼다면 OnTriggerEnter(Collider other)를 사용하세요.

        if (collision.gameObject.tag == "PlayerBullet")
        {
            // 1. 부딪힌 플레이어 총알은 즉시 파괴
            Destroy(collision.gameObject);

            // ▼▼▼ (수정) 체력 감소 로직 ▼▼▼
            // 2. 나의 체력을 1 감소
            health--;

            // 3. 내 체력이 0 이하가 되면 나 자신(보스 총알)을 파괴
            if (health <= 0)
            {
                Destroy(gameObject);
                // (선택 사항) 여기에 작은 폭발 이펙트나 사운드를 추가할 수도 있습니다.
            }
            // ▲▲▲ (수정) 체력 감소 로직 ▲▲▲
        }

         
         if (collision.gameObject.tag == "Player")
        {
            // 플레이어에게 데미지를 주고
            // collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);

            // 나 자신은 파괴
            Destroy(gameObject);
        }
    }
}