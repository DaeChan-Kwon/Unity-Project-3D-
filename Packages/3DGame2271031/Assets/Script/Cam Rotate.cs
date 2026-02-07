using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 20f;
    float mx =0;
    float my = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        mx += mouse_X * rotSpeed;          // * Time.deltaTime 제거
        my += mouse_Y * rotSpeed;          // * Time.deltaTime 제거

        my = Mathf.Clamp(my, -90f, 90f);
        transform.eulerAngles = new Vector3(-my, mx, 0);
    }

}
