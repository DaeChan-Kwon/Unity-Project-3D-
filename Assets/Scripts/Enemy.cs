using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    // --- 기본 변수 ---
    public float speed = 5;                    // 이동 속도
    public int health = 3;                     // 기본 체력

    // --- 점수 1500점 넘으면 능력치 강화 변수 ---
    public float buffedSpeed = 8f;             // 강화된 이동 속도
    public int buffedHealth = 5;                // 강화된 체력
    public int difficultyScoreThreshold = 1500; // 난이도 상승 기준 점수

    public GameObject effect;                   // 적 사망 시 이펙트
    public AudioClip explosionSound;            // 적 사망 시 폭발음

    // --- 총알 발사 변수 ---
    public GameObject enemyBulletFactory;      // 적 총알 프리팹
    public Transform firePosition;              // 총알 발사 위치
    public float fireRate = 2f;                 // 발사 간격(초)
    public float timeBetweenShots = 0.3f;      // 연속 발사 간격
    private float fireTimer;                    // 발사 타이머
    public AudioClip hitSound;                  // 적 피격 사운드
    public Renderer modelToFlash;               // 깜빡임 처리할 모델 렌더러
    private Color originalColor;                // 모델 원래 색상 저장

    // --- 점수 매니저 변수 ---
    private ScoreManager scoreManager;

    void Start()
    {
        // 씬에서 ScoreManager 오브젝트 찾아서 참조 얻기
        GameObject smObject = GameObject.Find("ScoreManager");
        if (smObject != null)
        {
            scoreManager = smObject.GetComponent<ScoreManager>();
        }
        else
        {
            Debug.LogError("Enemy 스크립트: 'ScoreManager' 오브젝트를 씬에서 찾을 수 없습니다!");
        }

        // 점수가 기준 이상이면 적 능력치 강화
        if (scoreManager != null && scoreManager.currentScore > difficultyScoreThreshold)
        {
            speed = buffedSpeed;
            health = buffedHealth;
        }

        fireTimer = fireRate;

        // 모델 렌더러 색상 저장
        if (modelToFlash != null)
        {
            originalColor = modelToFlash.material.color;
        }
        else
        {
            Debug.LogWarning("Enemy 프리팹에 'Model To Flash'가 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        // 적이 아래 방향으로 일정 속도로 이동
        transform.position += Vector3.down * speed * Time.deltaTime;

        // 총알 발사 타이머 감소 및 발사
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            StartCoroutine(FireBurst());
            fireTimer = fireRate;
        }
    }

    // 3발 연속 총알 발사 코루틴
    IEnumerator FireBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            if (enemyBulletFactory != null && firePosition != null)
            {
                Instantiate(enemyBulletFactory, firePosition.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    // 플레이어 총알과 충돌 시 데미지 처리
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            Destroy(collision.gameObject);
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0) return;

        health -= damage;

        // 피격 사운드 재생
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // 빨간색 깜빡임 효과
        if (modelToFlash != null)
        {
            StartCoroutine(FlashEffect(Color.red, 0.1f));
        }

        // 체력 0 이하 시 사망 처리
        if (health <= 0)
        {
            // 점수 증가 및 UI 업데이트
            if (scoreManager != null)
            {
                scoreManager.currentScore += 20;
                scoreManager.currentUI.text = "Score : " + scoreManager.currentScore;

                // 최고 점수 갱신 처리
                if (scoreManager.currentScore > scoreManager.bestScore)
                {
                    scoreManager.bestScore = scoreManager.currentScore;
                    scoreManager.bestUI.text = "Best Score : " + scoreManager.bestScore;
                    PlayerPrefs.SetInt("Best Score", scoreManager.bestScore);
                    PlayerPrefs.Save();
                }
            }

            // 폭발 이펙트 및 사운드 재생
            Instantiate(effect, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position);

            Destroy(gameObject);
        }
    }

    // 깜빡임 효과 코루틴
    IEnumerator FlashEffect(Color flashColor, float duration)
    {
        if (modelToFlash != null)
        {
            modelToFlash.material.color = flashColor;
            yield return new WaitForSeconds(duration);
            modelToFlash.material.color = originalColor;
        }
    }
}
