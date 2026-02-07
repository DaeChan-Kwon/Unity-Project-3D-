using UnityEngine;

public class JoystickMove : MonoBehaviour
{
    public float speed = 2;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        float horizontal = variableJoystick.Horizontal;

        Vector3 direction = new Vector3(horizontal, 0, 0).normalized;
        rb.linearVelocity = direction * speed;

    }
}
