using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Playermove : MonoBehaviour
{
    public GameObject playerExplosionEffect; // 플레이어 사망 시 생성할 폭발 이펙트
    public GameObject gameOverUI;             // 게임 오버 UI 객체
    public AudioSource bgmSource;             // 배경음악 오디오 소스
    public ScoreManager scoreManager;         // 점수 관리자 스크립트 참조
    public int health = 5;                    // 현재 플레이어 체력
    public int maxHealth = 5;                 // 최대 체력
    public Image healthBar;                   // 체력바 UI 이미지
    public AudioClip hitSound;                // 피격 시 재생할 사운드
    public Transform mainCamera;              // 게임 내 주요 카메라 위치 참조
    private Vector3 cameraOriginalPos;        // 카메라 원래 위치 저장 변수

    public Renderer modelRenderer; // 플레이어 모델 렌더러, 인스펙터에서 연결 가능

    private Color originalColor; // 원래 모델 색상 저장용

    void Start()
    {
        Time.timeScale = 1f; // 게임 시간 정상화

        // 인스펙터에서 modelRenderer가 지정 안 되어 있을 시 자동 탐색
        if (modelRenderer == null)
        {
            modelRenderer = GetComponentInChildren<Renderer>();
        }

        if (modelRenderer != null)
        {
            originalColor = modelRenderer.material.color; // 원래 색상 저장
            Debug.Log("플레이어 모델(Renderer)을 성공적으로 찾았습니다: " + modelRenderer.gameObject.name);
        }
        else
        {
            Debug.LogError("오류: 플레이어 모델(Renderer)을 찾을 수 없습니다! 깜빡임 효과가 작동하지 않습니다.");
        }

        if (mainCamera != null)
        {
            cameraOriginalPos = mainCamera.position; // 카메라 원위치 저장
        }

        UpdateHealthBar(); // 시작 시 체력바 갱신
    }

    void Update()
    {
        // 플레이어 이동 처리: 수평, 수직 입력 받아서 이동
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = Vector3.right * h + Vector3.up * v;
        transform.Translate(dir * 10 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 적 혹은 적 총알과 충돌 시 처리
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(collision.gameObject); // 충돌한 적 또는 총알 파괴
            health--;                      // 체력 감소
            UpdateHealthBar();             // 체력바 갱신

            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position); // 피격 사운드 재생
            }
            if (modelRenderer != null)
            {
                StartCoroutine(FlashEffect(Color.red, 0.1f)); // 피격 깜빡임 효과
            }
            if (mainCamera != null)
            {
                StartCoroutine(ShakeCamera(0.2f, 0.1f)); // 카메라 흔들림 효과
            }

            // 체력 0 이하 시 플레이어 사망 처리
            if (health <= 0)
            {
                Debug.Log("플레이어 사망!");
                if (playerExplosionEffect != null)
                {
                    Instantiate(playerExplosionEffect, transform.position, Quaternion.identity); // 폭발 이펙트 생성
                }
                if (bgmSource != null) bgmSource.Stop(); // 배경음악 정지
                if (gameOverUI != null) gameOverUI.SetActive(true); // 게임오버 UI 활성화
                Time.timeScale = 0f; // 게임 일시정지
                if (scoreManager != null) { scoreManager.TriggerGameOver(); } // 게임오버 상태 알림
                Destroy(gameObject); // 플레이어 오브젝트 파괴
            }
        }
    }

    // 깜빡임 효과 코루틴 (색상 변경, 일정 시간 후 원래 색으로 복구)
    public IEnumerator FlashEffect(Color flashColor, float duration)
    {
        if (modelRenderer != null)
        {
            modelRenderer.material.color = flashColor;
            yield return new WaitForSeconds(duration);
            modelRenderer.material.color = originalColor;
        }
    }

    // 카메라 흔들림 효과 코루틴
    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            mainCamera.position = new Vector3(cameraOriginalPos.x + x, cameraOriginalPos.y + y, cameraOriginalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.position = cameraOriginalPos; // 원위치 복구
    }

    // 체력바 UI 갱신 함수
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)health / maxHealth; // 체력 비율에 따른 fillAmount 설정
            healthBar.color = Color.Lerp(Color.red, Color.green, (float)health / maxHealth); // 체력에 따라 색상 변경 (빨강->초록)
        }
    }

    // 외부에서 호출하는 체력 회복 함수
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) { health = maxHealth; } // 최대 체력 초과 방지
        UpdateHealthBar(); // 체력바 갱신
        if (modelRenderer != null)
        {
            StartCoroutine(FlashEffect(Color.green, 0.1f)); // 회복 깜빡임 효과(초록색)
        }
    }
}
