using UnityEngine;

public class enemymanager : MonoBehaviour
{
    float currentTime = 0;
    public float createTime = 2;
    public GameObject enemy;

    float minTime = 1;
    float maxTime = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createTime = Random.Range(minTime, maxTime);   
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > createTime)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
            currentTime = 0;
            createTime = Random.Range(minTime, maxTime);

        }
    }
}
