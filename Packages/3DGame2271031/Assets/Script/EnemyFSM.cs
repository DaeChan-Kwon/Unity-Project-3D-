using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    EnemyState m_State;

    // 기본 설정
    Transform player;
    public float detectDistance = 8f;
    public float loseDistance = 30f;
    public float moveDistance = 20f;

    // 전투
    public float moveSpeed = 4f;
    public float attackDistance = 2f;
    public int attackPower = 3;
    float attackDelay = 2f;
    float currentTime;

    // 배회
    public float idleWaitTime = 1.5f;
    public float idleMoveRadius = 3f;
    float idleTimer;
    Vector3 idleTarget;
    bool isIdleMoving;

    // 체력
    public int maxHp = 15;
    public int hp;
    public Slider hpSlider;

    // 이펙트
    public GameObject bloodEffect;
    public GameObject groundBloodEffect;

    // 중력
    public float gravity = 20f;
    Vector3 velocity;

    CharacterController cc;
    Animator anim;
    Vector3 originPos;
    Quaternion originRot;

    // 사운드
    public AudioSource audioSource;
    public AudioClip[] idleSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] dieSounds;
    public float idleSoundMinInterval = 3f;
    public float idleSoundMaxInterval = 8f;
    private float idleSoundTimer;

    void Start()
    {
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        originPos = transform.position;
        originRot = transform.rotation;
        hp = maxHp;
        idleTimer = idleWaitTime;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        idleSoundTimer = Random.Range(idleSoundMinInterval, idleSoundMaxInterval);
    }

    void Update()
    {
        if (!cc.enabled || m_State == EnemyState.Die) return;

        switch (m_State)
        {
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Move: Move(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Return: Return(); break;
        }

        ApplyGravity();

        if (hpSlider != null) hpSlider.value = (float)hp / maxHp;
    }

    // 배회 상태
    void Idle()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer < detectDistance)
        {
            m_State = EnemyState.Move;
            anim.SetTrigger("idletoMove");
            isIdleMoving = false;
            return;
        }

        // Idle 사운드 재생
        idleSoundTimer -= Time.deltaTime;
        if (idleSoundTimer <= 0f && idleSounds.Length > 0)
        {
            PlayRandomSound(idleSounds);
            idleSoundTimer = Random.Range(idleSoundMinInterval, idleSoundMaxInterval);
        }

        // 랜덤 배회
        if (!isIdleMoving)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                Vector3 rand = Random.insideUnitSphere * idleMoveRadius;
                rand.y = 0f;
                idleTarget = originPos + rand;
                isIdleMoving = true;
            }
        }
        else
        {
            Vector3 dir = (idleTarget - transform.position).normalized;
            dir.y = 0f;
            cc.Move(dir * (moveSpeed * 0.5f) * Time.deltaTime);
            transform.forward = dir;

            if (Vector3.Distance(transform.position, idleTarget) < 0.2f)
            {
                isIdleMoving = false;
                idleTimer = idleWaitTime;
            }
        }
    }

    // 추격 상태
    void Move()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distToOrigin = Vector3.Distance(transform.position, originPos);

        if (distToPlayer > loseDistance || distToOrigin > moveDistance)
        {
            m_State = EnemyState.Return;
            return;
        }

        if (distToPlayer > attackDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;
        }
        else
        {
            m_State = EnemyState.Attack;
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }
    }

    // 공격 상태
    void Attack()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackDistance)
        {
            m_State = EnemyState.Move;
            currentTime = 0f;
            return;
        }

        currentTime += Time.deltaTime;
        if (currentTime >= attackDelay)
        {
            player.GetComponent<Playermove>().DamageAction(attackPower);

            if (attackSounds.Length > 0) PlayRandomSound(attackSounds);

            currentTime = 0f;
            anim.SetTrigger("StartAttack");
        }
    }

    // 복귀 상태
    void Return()
    {
        float dist = Vector3.Distance(transform.position, originPos);
        if (dist > 0.1f)
        {
            Vector3 dir = (originPos - transform.position).normalized;
            dir.y = 0f;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;
        }
        else
        {
            transform.position = originPos;
            transform.rotation = originRot;
            hp = maxHp;
            m_State = EnemyState.Idle;
            idleTimer = idleWaitTime;
            anim.SetTrigger("MoveToIdle");
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
    public void HitEnemy(int power, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (m_State == EnemyState.Die || m_State == EnemyState.Return) return;

        hp -= power;
        SpawnBodyBlood(hitPoint, hitNormal);
        SpawnGroundBlood(hitPoint);

        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            StartCoroutine(DamageProcess());
        }
        else
        {
            m_State = EnemyState.Die;
            Die();
        }
    }
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(0.3f);
        m_State = EnemyState.Move;
    }

    // 사망 처리
    void Die()
    {
        anim.SetTrigger("Die");

        if (dieSounds.Length > 0) PlayRandomSound(dieSounds);

        cc.enabled = false;

        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null) spawner.OnEnemyDead();

        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        if (hpSlider != null) hpSlider.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // 혈흔 이펙트 생성
    void SpawnBodyBlood(Vector3 pos, Vector3 normal)
    {
        if (!bloodEffect) return;

        GameObject b = Instantiate(bloodEffect, pos, Quaternion.LookRotation(normal));
        b.transform.SetParent(transform);
        Destroy(b, 2f);
    }

    void SpawnGroundBlood(Vector3 hitPoint)
    {
        if (!groundBloodEffect) return;

        RaycastHit hit;
        if (Physics.Raycast(hitPoint + Vector3.up * 0.2f, Vector3.down, out hit, 20f))
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            GameObject g = Instantiate(groundBloodEffect, hit.point + hit.normal * 0.05f, rot);
            Destroy(g, 5f);
        }
    }

    // 사운드 감지 (외부 호출용)
    public void OnHeardSound(Vector3 soundPos)
    {
        if (m_State == EnemyState.Die) return;
        m_State = EnemyState.Move;
    }

    // 랜덤 사운드 재생
    void PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || audioSource == null) return;

        int randomIndex = Random.Range(0, clips.Length);
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clips[randomIndex]);
    }
}