using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerfire : MonoBehaviour
{
    public GameObject bulletFactory;

    public GameObject firePosition;

    public float fireRate = 0.5f;
    float lastTime = 0;
    public int feverScore = 100;
    public float buffTime = 0f;

    public TextMeshProUGUI feverUI;

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
        if (sm.currentScore >= feverScore)
        {
            feverScore += 100;
            buffTime = Time.time + 3f;
            fireRate = 0.25f;
        }
        //if(Input.GetButtonDown("Jump"))
        if(buffTime > Time.time && Time.time - lastTime >= fireRate)
        {
            feverUI.text = "FEVER!";
            GameObject bullet = Instantiate(bulletFactory);
            bullet.transform.position = firePosition.transform.position;
            lastTime = Time.time;
        }
        else if(Time.time - lastTime >= fireRate)
        {
            feverUI.text = "";
            fireRate = 0.5f;
            GameObject bullet = Instantiate(bulletFactory);
            bullet.transform.position = firePosition.transform.position;
            lastTime = Time.time;
        }
    }
}
