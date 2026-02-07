using UnityEngine;
using UnityEngine.UI;

public class BombThrow : MonoBehaviour
{
    // 투척 설정
    public GameObject bombFactory;
    public Transform bombFirePosition;
    public float throwPower = 18f;
    public float upwardOffset = 2f; // 포물선 각도

    // 쿨타임
    public float bombCooldown = 15f;
    private float nextBombTime = 0f;

    // UI
    public Image bombIconImage;
    [Range(0f, 1f)] public float readyAlpha = 1f;
    [Range(0f, 1f)] public float cooldownAlpha = 0.3f;

    // 애니메이션
    public Animator gunAnimator;
    public string throwAnimationTrigger = "Throw";

    // 오디오
    public AudioClip throwSound;
    [Range(0.5f, 3f)] public float throwVolume = 1.5f;
    private AudioSource audioSource;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null) Debug.LogError("Main Camera not found!");

        // AudioSource 생성
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D 사운드
        audioSource.volume = 1f;

        if (gunAnimator == null) gunAnimator = GetComponentInChildren<Animator>();

        UpdateBombUI();
    }

    void Update()
    {
        UpdateBombUI();

        // 우클릭으로 폭탄 투척
        if (Input.GetMouseButtonDown(1))
        {
            TryThrowBomb();
        }
    }

    // 폭탄 투척 시도
    private void TryThrowBomb()
    {
        if (Time.time < nextBombTime)
        {
            Debug.Log($"폭탄 쿨타임: {nextBombTime - Time.time:F1}초 남음");
            return;
        }

        if (bombFactory == null || playerCamera == null)
        {
            Debug.LogError("Bomb Factory 또는 Camera가 없습니다!");
            return;
        }

        // 사운드 재생
        if (throwSound != null) audioSource.PlayOneShot(throwSound, throwVolume);

        // 애니메이션 재생
        gunAnimator?.SetTrigger(throwAnimationTrigger);

        // 투척 위치 설정
        Vector3 spawnPos = bombFirePosition != null
            ? bombFirePosition.position
            : transform.position + transform.forward * 0.5f + Vector3.up;

        // 폭탄 생성 및 던지기
        GameObject bomb = Instantiate(bombFactory, spawnPos, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Bomb Prefab에 Rigidbody가 없습니다!");
            return;
        }

        // 투척 방향 계산 (카메라 방향 + 위쪽)
        Vector3 throwDirection = playerCamera.transform.forward + Vector3.up * upwardOffset * 0.1f;
        rb.AddForce(throwDirection.normalized * throwPower, ForceMode.Impulse);

        // 쿨타임 시작
        nextBombTime = Time.time + bombCooldown;
    }

    // UI 투명도 업데이트 (쿨타임 표시)
    private void UpdateBombUI()
    {
        if (bombIconImage == null) return;

        bool isReady = Time.time >= nextBombTime;
        Color c = bombIconImage.color;
        c.a = isReady ? readyAlpha : cooldownAlpha;
        bombIconImage.color = c;
    }
}