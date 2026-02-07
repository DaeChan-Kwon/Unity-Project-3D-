using UnityEngine;

public class Playerfire : MonoBehaviour
{
    public GameObject firePosition;
    public GameObject bombFactory;
    public float throwPower = 15f;
    public GameObject bulletEffect;
    ParticleSystem ps;
    public int weaponPower = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            GameObject bomb = Instantiate(bombFactory);
            bomb.transform.position = firePosition.transform.position;
            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitinfo = new RaycastHit();
            if(Physics.Raycast(ray, out hitinfo))
            {
                if (hitinfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitinfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(weaponPower);
                }
                else
                {
                    bulletEffect.transform.position = hitinfo.point;
                    bulletEffect.transform.forward = hitinfo.normal;
                    ps.Play();
                }
                    
            }
        }

    }
}
