using UnityEngine;

public class Playerfire : MonoBehaviour
{
    // 발사 설정
    public GameObject firePosition;
    public GameObject bulletEffect;
    ParticleSystem ps;
    public int weaponPower = 5;
    public float fireRate = 0.25f;
    private float nextFireTime = 0f;

    // 애니메이션
    public Animator gunAnimator;
    public string fireAnimationTrigger = "Fire";

    void Start()
    {
        if (bulletEffect != null) ps = bulletEffect.GetComponent<ParticleSystem>();
        if (gunAnimator == null) gunAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 마우스 클릭 시 연사 속도에 맞춰 발사
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        if (Camera.main == null) return;

        // 발사 애니메이션 재생
        if (gunAnimator != null) gunAnimator.SetTrigger(fireAnimationTrigger);

        // 레이캐스트로 명중 판정
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            // 적 레이어 명중 시 각 적 타입별 처리
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyFSM enemy1 = hit.collider.GetComponentInParent<EnemyFSM>();
                if (enemy1 != null)
                {
                    enemy1.HitEnemy(weaponPower, hit.point, hit.normal);
                    return;
                }

                EnemyFSM2 enemy2 = hit.collider.GetComponentInParent<EnemyFSM2>();
                if (enemy2 != null)
                {
                    enemy2.HitEnemy(weaponPower, hit.point, hit.normal);
                    return;
                }

                BossFSM boss = hit.collider.GetComponentInParent<BossFSM>();
                if (boss != null)
                {
                    boss.HitEnemy(weaponPower, hit.point, hit.normal);
                    return;
                }
            }
            else
            {
                // 벽/바닥 명중 시 이펙트 재생
                if (bulletEffect != null && ps != null)
                {
                    bulletEffect.transform.position = hit.point + hit.normal * 0.01f;
                    bulletEffect.transform.rotation = Quaternion.LookRotation(hit.normal);
                    ps.Play();
                }
            }
        }
    }
}