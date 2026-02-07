using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;

public class playermove : MonoBehaviour
{
    pausemanager pm;
    scoremanager sm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject pmObject = GameObject.Find("pausemanager");
        pm = pmObject.GetComponent<pausemanager>();
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //Debug.Log(h +  ", " + v);
        if (transform.position.x >= 3f && h > 0)
        {
            h = 0;
        }
        if (transform.position.x <= -3f && h < 0)
        {
            h = 0;
        }
        if (transform.position.y >= 4.5f && v > 0)
        {
            v = 0;
        }
        if (transform.position.y <= -4.5f && v < 0)
        {
            v = 0;
        }

        Vector3 dir = Vector3.right * h + Vector3.up * v;
        transform.Translate(dir * 5 * Time.deltaTime);

    }
    private void OnCollisionEnter(Collision collision)
    {
        pausemanager.isPaused = true;
        Time.timeScale = 0f;
        pm.pauseUI.text = "GAME OVER";
        pm.menuUI.text = "\n     Score: " + sm.currentScore + "\nRESTART:SPACE";
    }
}
