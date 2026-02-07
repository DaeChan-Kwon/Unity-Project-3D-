using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int healAmount = 1; // 플레이어가 회복할 체력 양
    public float speed = 3f;   // 힐팩이 화면 아래로 떨어지는 속도

    void Update()
    {
        // 힐팩을 매 프레임마다 아래 방향으로 이동시킴 (시간에 따라 속도 조절)
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 상대가 "Player" 태그인지 확인
        if (other.CompareTag("Player"))
        {
            // 충돌한 오브젝트에서 플레이어 스크립트(Playermove) 컴포넌트 가져오기
            Playermove player = other.GetComponent<Playermove>();
            if (player != null)
            {
                // 플레이어의 Heal 함수 호출하여 체력 회복 (깜빡임 효과 등은 플레이어 스크립트가 처리)
                player.Heal(healAmount);
            }

            // 힐팩 아이템 삭제(획득 후 사라짐)
            Destroy(gameObject);
        }
    }
}
