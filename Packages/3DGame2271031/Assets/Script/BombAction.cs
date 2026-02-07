using UnityEngine;
using System.Collections;

public class BombAction : MonoBehaviour
{
    // 폭발 설정
    public GameObject bombEffect;
    public float explosionRadius = 5.0f;
    public float explosionForce = 700.0f;
    public LayerMask ignoreLayers;
    public int explosionDamage = 30;

    // 오디오
    public AudioClip explosionSound;
    [Range(1f, 5f)] public float explosionVolume = 3.0f;

    private AudioSource audioSource;
    private bool hasExploded = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Bomb에 AudioSource가 없습니다!");
            return;
        }

        // 2D 사운드로 설정
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
    }
    void Start()
    {
        if (bombEffect != null && bombEffect.transform.IsChildOf(transform))
            bombEffect.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어나 총에 닿으면 무시
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Player") ||
            collision.gameObject.name.Contains("Rifle") ||
            collision.gameObject.name.Contains("Gun"))
            return;

        if (!hasExploded) Explode();
    }
  private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // 폭발음 재생
        if (explosionSound != null && audioSource != null)
            audioSource.PlayOneShot(explosionSound, explosionVolume);

        // 폭발 이펙트 생성
        if (bombEffect != null)
        {
            GameObject eff;
            if (bombEffect.transform.IsChildOf(transform))
            {
                bombEffect.SetActive(true);
                bombEffect.transform.SetParent(null);
                eff = bombEffect;
            }
            else
            {
                eff = Instantiate(bombEffect, transform.position, Quaternion.identity);
            }
            Destroy(eff, 3f);
        }

        // 폭발 범위 내 오브젝트 탐지
        int layerMask = ~ignoreLayers.value;
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player")) continue;

            // 물리 효과 적용
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);

            // 적 데미지 처리
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Vector3 hitPoint = hit.ClosestPoint(transform.position);
                Vector3 hitNormal = (hitPoint - transform.position).normalized;

                EnemyFSM e1 = hit.GetComponentInParent<EnemyFSM>();
                if (e1 != null)
                {
                    e1.HitEnemy(explosionDamage, hitPoint, hitNormal);
                    continue;
                }

                EnemyFSM2 e2 = hit.GetComponentInParent<EnemyFSM2>();
                if (e2 != null) e2.HitEnemy(explosionDamage, hitPoint, hitNormal);

                BossFSM boss = hit.GetComponentInParent<BossFSM>();
                if (boss != null) boss.HitEnemy(explosionDamage, hitPoint, hitNormal);
            }
        }

        // 폭발음이 끝난 후 오브젝트 파괴
        if (explosionSound != null)
        {
            StartCoroutine(DestroyAfterSound(explosionSound.length + 0.5f));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 사운드 재생 시간 대기 후 파괴
    IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 폭발 범위 시각화 (에디터)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}