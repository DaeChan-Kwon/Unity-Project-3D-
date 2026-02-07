using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossFSM : MonoBehaviour
{
    enum BossState
    {
        Move,
        Attack,
        Damaged,
        Die
    }
    BossState m_State;

    // 기본 설정
    Transform player;
    public float moveSpeed = 3f;
    public float attackDistance = 3f;
    public float attackDelay = 3f;
    public int attackPower = 10;

    // 체력
    public int maxHp = 100;
    public Slider hpSlider;
    int hp;

    // 이펙트
    public GameObject bloodEffect;
    public GameObject groundBloodEffect;

    // 중력
    public float gravity = 20f;
    Vector3 velocity;

    // 열쇠 드롭
    public GameObject keyPrefab;
    public Vector3 dropOffset = new Vector3(0, 1, 0);

    CharacterController cc;
    Animator anim;
    float currentTime = 0f;

    // 사운드
    public AudioSource audioSource;
    public AudioClip[] moveSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] dieSounds;
    public float moveSoundMinInterval = 3f;
    public float moveSoundMaxInterval = 7f;
    private float moveSoundTimer;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        hp = maxHp;
        m_State = BossState.Move;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        moveSoundTimer = Random.Range(moveSoundMinInterval, moveSoundMaxInterval);
    }

    void Update()
    {
        if (cc == null || !cc.enabled || m_State == BossState.Die) return;

        switch (m_State)
        {
            case BossState.Move: Move(); break;
            case BossState.Attack: Attack(); break;
        }

        ApplyGravity();

        if (hpSlider != null) hpSlider.value = (float)hp / maxHp;
    }

    // 이동 상태
    void Move()
    {
        if (!cc.enabled) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer > attackDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;

            // 이동 사운드
            if (moveSounds.Length > 0)
            {
                moveSoundTimer -= Time.deltaTime;
                if (moveSoundTimer <= 0f)
                {
                    PlayRandomSound(moveSounds);
                    moveSoundTimer = Random.Range(moveSoundMinInterval, moveSoundMaxInterval);
                }
            }
        }
        else
        {
            m_State = BossState.Attack;
            currentTime = attackDelay;
            if (anim != null) anim.SetTrigger("MoveToAttackDelay");
        }
    }

    // 공격 상태
    void Attack()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer > attackDistance)
        {
            m_State = BossState.Move;
            currentTime = 0;
            return;
        }

        currentTime += Time.deltaTime;

        if (currentTime > attackDelay)
        {
            Playermove playerMove = player.GetComponent<Playermove>();
            if (playerMove != null) playerMove.DamageAction(attackPower);

            if (attackSounds.Length > 0) PlayRandomSound(attackSounds);

            currentTime = 0;
            if (anim != null) anim.SetTrigger("StartAttack");
        }
    }

    // 중력 적용
    void ApplyGravity()
    {
        if (cc.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        else
            velocity.y -= gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }

    // 피격 처리
    public void HitEnemy(int hitPower, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (m_State == BossState.Die) return;

        hp -= hitPower;
        SpawnBodyBlood(hitPoint, hitNormal);
        SpawnGroundBlood(hitPoint);

        if (hp > 0)
        {
            m_State = BossState.Damaged;
            if (anim != null) anim.SetTrigger("Hit");
            StartCoroutine(DamageProcess());
        }
        else
        {
            m_State = BossState.Die;
            Die();
        }
    }

    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(0.5f);
        m_State = BossState.Move;
    }

    // 사망 처리
    void Die()
    {
        if (anim != null) anim.SetTrigger("Die");

        if (dieSounds.Length > 0) PlayRandomSound(dieSounds);

        cc.enabled = false;

        DropKey();
        StartCoroutine(DieProcess());
    }

    // 열쇠 드롭
    void DropKey()
    {
        if (keyPrefab != null)
        {
            Vector3 dropPos = transform.position + dropOffset;
            Instantiate(keyPrefab, dropPos, Quaternion.identity);
        }
    }

    IEnumerator DieProcess()
    {
        if (hpSlider != null) hpSlider.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // 혈흔 이펙트
    void SpawnBodyBlood(Vector3 pos, Vector3 normal)
    {
        if (bloodEffect == null) return;
        GameObject blood = Instantiate(bloodEffect, pos, Quaternion.LookRotation(normal));
        blood.transform.SetParent(transform);
        Destroy(blood, 2f);
    }

    void SpawnGroundBlood(Vector3 hitPoint)
    {
        if (groundBloodEffect == null) return;
        RaycastHit hit;
        if (Physics.Raycast(hitPoint + Vector3.up * 0.2f, Vector3.down, out hit, 20f))
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            GameObject groundBlood = Instantiate(groundBloodEffect, hit.point + hit.normal * 0.05f, rot);
            Destroy(groundBlood, 5f);
        }
    }

    // 랜덤 사운드 재생
    void PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || audioSource == null) return;

        int index = Random.Range(0, clips.Length);
        audioSource.pitch = Random.Range(0.92f, 1.08f);
        audioSource.PlayOneShot(clips[index]);
    }
}