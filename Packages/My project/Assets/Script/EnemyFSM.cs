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
    public float findDistance = 8f;
    Transform player;

    public float moveSpeed = 4f;
    public float attackDistance = 2f;
    CharacterController cc;
    float currentTime = 0;
    float attackDelay = 2f;
    public int attackPower = 3;
    Vector3 originPos;
    Quaternion originRot;
    public float moveDistance = 20f;
    public int hp;
    public int maxHp = 15;
    public Slider hpSlider;
    Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
        hp = maxHp;
        anim = transform.GetComponentInChildren<Animator>();

    }


    // Update is called once per frame
    void Update()
    {
        switch(m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
               // Damaged();
                break;
            case EnemyState.Die:
               // Die();
                break;
        }
        Debug.Log(hp);
        hpSlider.value = (float)hp / (float)maxHp;
    }
    void Idle()
    {
        if(Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            print("상태 전환 : idle -> Move");
            anim.SetTrigger("idletoMove");
        }
    }
    void Move()
    {
        if(Vector3.Distance(transform.position, originPos)>moveDistance)
        {
            m_State = EnemyState.Return;
            print("상태 전환 : Move -> Return");
        }
        // 만약 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // 이동 방향 설정
            Vector3 dir = (player.position - transform.position).normalized;
            // 플레이어를 향해 이동, 캐릭터 컨트롤러 컴포넌트 사용
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;
        }
        // 현재 상태를 공격 상태로 전환
        else
        {
            m_State = EnemyState.Attack;
            print("상태 전환: Move -> Attack");
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }

    }
    void Attack()
    {
        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어를 공격
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            // 일정한 시간마다 플레이어를 공격
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                player.GetComponent<Playermove>().DamageAction(attackPower);

                print("공격!");
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        // 현재 상태를 이동으로 전환, 재추격.
        else
        {
            m_State = EnemyState.Move;
            print("상태 전환: Attack -> Move");
            currentTime = 0;
        }

    }
    void Return()
    {
        // 만일 초기 위치에서의 거리가 0.1f 이상이라면 초기 위치 쪽으로 이동
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            Vector3 dir = (originPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;
        }
        // 적의 위치를 초기 위치로 조정하고 현재 상태를 대기로 전환
        else
        {
            transform.position = originPos;
            transform.rotation = originRot;
            hp = maxHp;
            m_State = EnemyState.Idle;
            print("상태 전환 : Return -> Idle");
            anim.SetTrigger("MoveToIdle");
        }

    }
    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(0.5f);
        m_State = EnemyState.Move;
        print("상태 전환 : Damaged -> Move");
    }
    public void HitEnemy(int hitPower)
    {
        if(m_State==EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        hp -= hitPower;
        // 적의 체력이 0보다 크면 Damage 상태로 전환
        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            print("상태 전환: Any Stat");
            Damaged();
        }
        else
        {
            m_State = EnemyState.Die;
            print("상태 전환: Any State -> Die");
            Die();
        }
    }
    void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러 컴포넌트 비활성화
        cc.enabled = false;
        // 일정 시간 대기 후 자기 자신을 제거
        yield return new WaitForSeconds(2f);
        print("적 소멸");
        Destroy(gameObject);
    }

}

