using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f; // "피하기 쉽게" 속도를 10 정도로 설정

    void Update()
    {
        // 그냥 아래로만 이동
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }
}