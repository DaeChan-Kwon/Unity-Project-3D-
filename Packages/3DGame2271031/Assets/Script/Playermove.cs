using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Playermove : MonoBehaviour
{
    // 이동 설정
    public float moveSpeed = 7f;
    public float runSpeed = 12f;
    public float jumpPower = 10f;
    public float runJumpMultiplier = 1.3f; // 달리기 중 점프력 배율
    private float currentMoveSpeed;

    // 중력 및 컨트롤러
    CharacterController cc;
    float gravity = -20f;
    float yVelocity = 0;
    public bool isJumping = false;
    private bool wasGrounded;
    private bool hasJumpedAndIsFalling = false;

    // 체력 관련
    public int hp = 20;
    int maxHp;
    public Slider hpSlider;
    public GameObject hitEffect;

    // 게임 오버
    public GameObject gameOverUI;
    public float autoEndDelay = 10f;
    private bool isDead = false;

    // 오디오
    public AudioSource audioSource;
    public AudioClip[] walkClips;
    public AudioClip[] runClips;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip gameOverSound;
    private float nextFootstepTime;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private string currentSceneName;

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        cc = GetComponent<CharacterController>();
        currentMoveSpeed = moveSpeed;
        wasGrounded = cc.isGrounded;

        maxHp = hp;
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = hp;
        }

        if (gameOverUI != null) gameOverUI.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        isDead = false;
    }

    void Update()
    {
        // 사망 또는 일시정지 상태면 동작 중단
        if (isDead || Time.timeScale == 0f)
        {
            if (cc.enabled) cc.Move(Vector3.zero);
            return;
        }

        // 이동 속도 설정 (Shift 입력 시 달리기)
        currentMoveSpeed = (Input.GetKey(KeyCode.LeftShift) && cc.isGrounded) ? runSpeed : moveSpeed;

        // 이동 입력 처리
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 horizontalDir = new Vector3(h, 0, v).normalized;
        bool isMoving = horizontalDir.magnitude > 0.1f;

        // 카메라 방향 기준으로 이동
        if (Camera.main != null)
        {
            horizontalDir = Camera.main.transform.TransformDirection(horizontalDir);
        }

        bool isGroundedNow = cc.isGrounded;

        // 착지 처리
        if (isGroundedNow)
        {
            isJumping = false;
            yVelocity = -0.5f;

            if (!wasGrounded && hasJumpedAndIsFalling)
            {
                PlayLandingSound();
                hasJumpedAndIsFalling = false;
            }
        }

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGroundedNow)
        {
            float finalJumpPower = jumpPower;
            if (currentMoveSpeed >= runSpeed - 0.5f) finalJumpPower *= runJumpMultiplier;

            yVelocity = finalJumpPower;
            isJumping = true;
            hasJumpedAndIsFalling = true;
            PlayJumpSound();
        }

        // 중력 적용
        yVelocity += gravity * Time.deltaTime;

        // 최종 이동
        Vector3 finalMove = horizontalDir * currentMoveSpeed;
        finalMove.y = yVelocity;
        cc.Move(finalMove * Time.deltaTime);

        // 발소리 재생
        if (isGroundedNow && isMoving && Time.time >= nextFootstepTime)
        {
            PlayFootstepSound();
        }

        wasGrounded = isGroundedNow;

        // HP 슬라이더 업데이트
        if (hpSlider != null) hpSlider.value = hp;
    }

    // 발소리 재생 (걷기/달리기 구분)
    private void PlayFootstepSound()
    {
        if (audioSource == null) return;

        AudioClip[] currentClips = (currentMoveSpeed >= runSpeed - 0.5f) ? runClips : walkClips;
        float currentInterval = (currentMoveSpeed >= runSpeed - 0.5f) ? runStepInterval : walkStepInterval;

        if (currentClips.Length == 0) return;

        audioSource.PlayOneShot(currentClips[Random.Range(0, currentClips.Length)]);
        nextFootstepTime = Time.time + currentInterval;
    }

    private void PlayJumpSound()
    {
        if (audioSource != null && jumpClip != null) audioSource.PlayOneShot(jumpClip);
    }

    private void PlayLandingSound()
    {
        if (audioSource != null && landClip != null) audioSource.PlayOneShot(landClip);
    }

    // 데미지 처리
    public void DamageAction(int damage)
    {
        if (isDead) return;

        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            StartGameOver();
        }
        else
        {
            StartCoroutine(PlayHitEffect());
        }
    }

    // 피격 이펙트 표시
    IEnumerator PlayHitEffect()
    {
        if (hitEffect == null) yield break;

        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitEffect.SetActive(false);
    }

    // 게임 오버 시작
    private void StartGameOver()
    {
        if (isDead) return;
        isDead = true;

        // BGM 페이드아웃
        if (BGMManager.instance != null) BGMManager.instance.StopBGMWithFade();

        // 게임 오버 사운드 재생
        if (audioSource != null && gameOverSound != null) audioSource.PlayOneShot(gameOverSound);

        Time.timeScale = 0f;

        if (gameOverUI != null) gameOverUI.SetActive(true);
        if (cc != null) cc.enabled = false;

        StartCoroutine(EndGameProcess());
    }

    // 게임 종료 프로세스 (스페이스 입력 또는 자동 종료)
    IEnumerator EndGameProcess()
    {
        float timer = 0f;

        while (timer < autoEndDelay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1f;
                AudioListener.volume = 1f;
                SceneManager.LoadScene(currentSceneName);
                yield break;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        AudioListener.volume = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}