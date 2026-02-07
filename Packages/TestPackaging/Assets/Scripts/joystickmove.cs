using UnityEngine;

public class joystickmove : MonoBehaviour
{
    public float speed = 5;
    public VariableJoystick variablejoystick;
    public Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = variablejoystick.Horizontal;
        float vertical = variablejoystick.Vertical;

        if (rb.position.x >= 3f && horizontal > 0)
        {
            horizontal = 0;
        }
        if (rb.position.x <= -3f && horizontal < 0)
        {
            horizontal = 0;
        }
        if (rb.position.y >= 4.5f && vertical > 0)
        {
            vertical = 0;
        }
        if (rb.position.y <= -4.5f && vertical < 0)
        {
            vertical = 0;
        }

        Vector3 direction = new Vector3(horizontal, vertical, 0).normalized;
        rb.linearVelocity = direction * speed;
    }
}
