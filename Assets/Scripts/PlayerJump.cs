using UnityEngine;

public class Jump : MonoBehaviour
{
    public float JumpPower = 5f;
    Rigidbody rb;
    bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            isJumping = true; 
        }
    }

  
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            isJumping = false;
        }
    }
}