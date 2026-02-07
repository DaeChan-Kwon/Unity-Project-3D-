using UnityEngine;

public class enemy : MonoBehaviour
{
    public float speed = 5;

    public int upgradeScore = 100;

    public GameObject effect;

    scoremanager sm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject smObject = GameObject.Find("scoremanager");
        sm = smObject.GetComponent<scoremanager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pausemanager.isPaused)
        {
            return;
        }
        if (sm.currentScore >= upgradeScore)
        {
            upgradeScore += 100;
            speed++;
        }

        Vector3 dir = Vector3.down;
        transform.position += dir * speed * Time.deltaTime;

        if (transform.position.y <= -5.5f)
        {
            sm.currentScore -= 5;
            sm.currentUI.text = "Score: " + sm.currentScore;
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {   
        sm.currentScore += 10;
        sm.currentUI.text = "Score: " + sm.currentScore;

        if(sm.currentScore > sm.bestScore)
        {
            sm.bestScore = sm.currentScore;
            sm.bestUI.text = "Best Score: " + sm.bestScore;

            PlayerPrefs.SetInt("Best Score", sm.bestScore);
        }

        Debug.Log("충돌 발생!");
        Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(collision.gameObject);
    }
}
