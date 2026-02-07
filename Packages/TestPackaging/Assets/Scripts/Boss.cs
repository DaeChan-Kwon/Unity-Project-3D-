using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    // --- 기본 변수 ---
    public float speed = 2;                   // 보스 이동 속도
    public GameObject effect;                 // 보스 사망 시 이펙트
    public int health = 50;                   // 보스 기본 체력
    public AudioClip hitSound;                // 피격 사운드
    public AudioClip explosionSound;         // 사망 폭발음

    public Renderer modelToFlash;             // 깜빡임 효과 적용할 모델 렌더러
    private Color originalColor;              // 원래 색상 저장

    // --- 총알 발사 변수 ---
    public GameObject bossBulletFactory;     // 보스 총알 프리팹
    public Transform firePosition;            // 총알 발사 위치
    public float fireRate = 3f;               // 총알 발사 간격
    public int bulletsPerBurst = 12;          // 한 번에 발사할 총알 개수
    private float fireTimer;                  // 발사 타이머

    // --- 보스 멈춤 위치 변수 ---
    public float stopYPosition = 7.0f;       // 보스가 멈출 y 좌표
    private bool hasReachedStopPosition = false; // 멈춤 위치 도달 여부

    // --- 강화 변수 ---
    public int buffedHealth = 75;             // 점수 1500점 넘었을 때 강화된 체력
    public int difficultyScoreThreshold = 1500; // 난이도 상승 기준 점수
    private ScoreManager scoreManager;        // 점수 관리자 참조

    void Start()
    {
        // 씬에서 ScoreManager 객체 찾아서 참조 획득
        GameObject smObject = GameObject.Find("ScoreManager");
        if (smObject != null)
        {
            scoreManager = smObject.GetComponent<ScoreManager>();
        }
        else
        {
            Debug.LogError("Boss 스크립트: 'ScoreManager' 오브젝트를 씬에서 찾을 수 없습니다!");
        }

        // 점수가 기준 이상일 경우 체력 강화
        if (scoreManager != null && scoreManager.currentScore > difficultyScoreThreshold)
        {
            Debug.Log("강화된 보스 출현!");
            this.health = buffedHealth;
        }

        // 모델 렌더러 색상 초기화
        if (modelToFlash != null)
        {
            originalColor = modelToFlash.material.color;
        }
        else
        {
            Debug.LogWarning("[translate:Boss 프리팹에 'Model To Flash'가 연결되지 않았습니다!]", this);
        }

        fireTimer = fireRate;
    }

    void Update()
    {
        // 보스가 멈춰야 할 위치까지 이동
        if (!hasReachedStopPosition)
        {
            if (transform.position.y > stopYPosition)
            {
                Vector3 dir = Vector3.down;
                transform.position += dir * speed * Time.deltaTime;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, stopYPosition, transform.position.z);
                hasReachedStopPosition = true;
            }
        }

        // 총알 발사 타이머 감소 및 radial burst 발사
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireRadialBurst();
            fireTimer = fireRate;
        }
    }

    // 360도 방향으로 총알을 균등하게 발사하는 함수
    void FireRadialBurst()
    {
        if (bossBulletFactory == null || firePosition == null)
        {
            Debug.LogError("[translate:BossBulletFactory 또는 FirePosition이 설정되지 않았습니다!]");
            return;
        }

        float angleStep = 360f / bulletsPerBurst;
        float currentAngle = 0f;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            Quaternion bulletRotation = Quaternion.Euler(0, 0, currentAngle);
            Instantiate(bossBulletFactory, firePosition.position, bulletRotation);
            currentAngle += angleStep;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어 총알과 충돌 시 데미지 처리
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

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

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
                scoreManager.currentScore += 100;
                scoreManager.currentUI.text = "Score : " + scoreManager.currentScore;
                if (scoreManager.currentScore > scoreManager.bestScore)
                {
                    scoreManager.bestScore = scoreManager.currentScore;
                    scoreManager.bestUI.text = "Best Score : " + scoreManager.bestScore;
                    PlayerPrefs.SetInt("Best Score", scoreManager.bestScore);
                    PlayerPrefs.Save();
                }
            }
            else
            {
                // Start()에서 못 찾았을 경우를 대비한 안전 코드
                GameObject smObject = GameObject.Find("ScoreManager");
                if (smObject != null)
                {
                    ScoreManager sm = smObject.GetComponent<ScoreManager>();
                    if (sm != null)
                    {
                        sm.currentScore += 100;
                        sm.currentUI.text = "Score : " + sm.currentScore;
                        if (sm.currentScore > sm.bestScore)
                        {
                            sm.bestScore = sm.currentScore;
                            sm.bestUI.text = "Best Score : " + sm.bestScore;
                            PlayerPrefs.SetInt("Best Score", sm.bestScore);
                            PlayerPrefs.Save();
                        }
                    }
                }
            }

            // 폭발 이펙트 및 폭발음 재생
            if (effect != null)
            {
                Instantiate(effect, transform.position, Quaternion.identity);
            }
            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position);
            }

            Destroy(gameObject);
        }
    }

    // 깜빡임 효과 구현 코루틴
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
