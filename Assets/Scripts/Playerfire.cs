using UnityEngine;

public class Playerfire : MonoBehaviour
{
    // === 1. 일반 총알/유도탄 관련 변수 ===
    public GameObject bulletFactory;          // 기본 총알 프리팹
    public float fireRate = 0.2f;              // 총알 발사 간격
    private float fireTimer = 0f;              // 발사 시간 간격 체크용 타이머
    public GameObject homingBulletFactory;    // 유도탄 총알 프리팹

    // === 2. 총알 발사 위치 ===
    public Transform firePositionLeft;         // 왼쪽 발사 위치
    public Transform firePositionRight;        // 오른쪽 발사 위치
    public Transform centerFirePosition;       // 중앙 발사 위치 (필살기용)

    // === 3. 점수 매니저 참조 ===
    public ScoreManager scoreManager;

    // === 필살기 관련 변수 ===
    public GameObject specialBulletFactory;   // 필살기 총알 프리팹
    public float specialCooldown = 10.0f;     // 필살기 쿨타임(초)
    private float specialCooldownTimer = 0f;  // 쿨타임 타이머
    private bool isCharging = false;           // 필살기 충전 상태

    // === 필살기 충전 이펙트 ===
    public GameObject chargingEffect;          // 충전 이펙트(파티클 등)

    // === 필살기 사운드 관련 ===
    private AudioSource chargeAudioSource;     // 충전 사운드용 오디오 소스
    public AudioClip chargeSound;               // 충전 소리 클립
    public AudioClip fireSound;                 // 발사 소리 클립

    void Start()
    {
        // 초기 쿨타임 타이머, 충전 상태 초기화
        specialCooldownTimer = 0f;
        isCharging = false;

        // 오디오소스 존재 여부 체크 및 없으면 자동 추가
        chargeAudioSource = GetComponent<AudioSource>();
        if (chargeAudioSource == null)
        {
            chargeAudioSource = gameObject.AddComponent<AudioSource>();
        }
        chargeAudioSource.playOnAwake = false;
        chargeAudioSource.loop = false;
        chargeAudioSource.Stop();

        // 게임 시작 시 충전 이펙트 비활성화
        if (chargingEffect != null)
        {
            chargingEffect.SetActive(false);
        }
    }

    void Update()
    {
        // 일반 총알 발사 처리 (스페이스바)
        fireTimer += Time.deltaTime;
        if (fireTimer > fireRate)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                fireTimer = 0f;
                if (scoreManager != null && scoreManager.currentScore >= 1500)
                {
                    // 점수가 1500 이상이면 유도탄 발사
                    if (homingBulletFactory != null)
                    {
                        if (firePositionLeft != null) Instantiate(homingBulletFactory, firePositionLeft.position, Quaternion.identity);
                        if (firePositionRight != null) Instantiate(homingBulletFactory, firePositionRight.position, Quaternion.identity);
                    }
                }
                else
                {
                    // 기본 총알 발사
                    if (bulletFactory != null)
                    {
                        if (firePositionLeft != null) Instantiate(bulletFactory, firePositionLeft.position, Quaternion.identity);
                        if (firePositionRight != null) Instantiate(bulletFactory, firePositionRight.position, Quaternion.identity);
                    }
                }
            }
        }

        // 필살기 쿨타임 감소 처리
        if (specialCooldownTimer > 0)
        {
            specialCooldownTimer -= Time.deltaTime;
        }

        // Shift 키 누르면 필살기 충전 시작
        if (Input.GetKeyDown(KeyCode.LeftShift) && specialCooldownTimer <= 0 && !isCharging)
        {
            isCharging = true;
            Debug.Log("필살기 충전 시작...");

            // 충전 이펙트 활성화
            if (chargingEffect != null) chargingEffect.SetActive(true);

            // 충전 소리 재생 (루프)
            if (chargeSound != null)
            {
                chargeAudioSource.clip = chargeSound;
                chargeAudioSource.loop = true;
                chargeAudioSource.Play();
            }
        }

        // Shift 키에서 손 떼면 필살기 발사
        if (Input.GetKeyUp(KeyCode.LeftShift) && isCharging)
        {
            isCharging = false;
            Debug.Log("필살기 발사!");

            // 충전 이펙트 비활성화
            if (chargingEffect != null) chargingEffect.SetActive(false);

            // 충전 소리 중지
            if (chargeAudioSource.isPlaying)
            {
                chargeAudioSource.Stop();
                chargeAudioSource.loop = false;
            }

            // 발사 소리 재생
            if (fireSound != null)
            {
                AudioSource.PlayClipAtPoint(fireSound, Camera.main.transform.position, 1.0f);
            }

            // 필살기 총알 생성 (중앙 위치에서)
            if (specialBulletFactory != null && centerFirePosition != null)
            {
                Instantiate(specialBulletFactory, centerFirePosition.position, Quaternion.identity);
            }

            // 쿨타임 타이머 초기화
            specialCooldownTimer = specialCooldown;
        }
    }
}
