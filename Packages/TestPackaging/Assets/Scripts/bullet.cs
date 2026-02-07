using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= 4.5f)
        {
            Destroy(gameObject);
        }
        Vector3 dir = Vector3.up;
        transform.position += dir * speed * Time.deltaTime; 
    }
}
