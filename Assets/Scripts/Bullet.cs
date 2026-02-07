using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;
   

    public GameObject bulletFactory;
    public GameObject firePosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Vector3.up;
        Debug.Log(dir);
        transform.position += dir * speed * Time.deltaTime;
    }
}
