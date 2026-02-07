using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;
    private void OnCollisionEnter(Collision collision)
    {
        GameObject eff = Instantiate(bombEffect);
        eff.transform.position = transform.position;
        Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
