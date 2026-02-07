using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        // ▼▼▼ 여기를 'GetAxisRaw'로 바꾸세요 ▼▼▼
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // ▲▲▲ 여기를 'GetAxisRaw'로 바꾸세요 ▲▲▲

        //Debug.Log(h + ", " + v);
        Vector3 dir = Vector3.right * h + Vector3.up * v;
        transform.Translate(dir * 10 * Time.deltaTime);
    }
}
