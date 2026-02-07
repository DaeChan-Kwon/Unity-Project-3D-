using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public float destroyTime = 1.5f;
    float currentTime = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime > destroyTime)
        {
            Destroy(gameObject);
        }
        currentTime += Time.deltaTime;
    }
}
