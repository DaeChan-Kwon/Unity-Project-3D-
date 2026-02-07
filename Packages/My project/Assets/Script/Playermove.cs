using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Playermove : MonoBehaviour
{
    public float moveSpeed = 7f;
    CharacterController cc;
    float gravity = -20f;
    float yVelocity = 0;
    public float jumpPower = 10f;
    public bool isJumping = false;

    public int hp = 20;
    int maxHp = 20;
    public Slider hpSlider;
    public GameObject hitEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
       
        dir = dir.normalized;
        dir = Camera.main.transform.TransformDirection(dir);
        transform.position += dir * moveSpeed * Time.deltaTime;
        
        if(isJumping && cc.collisionFlags == CollisionFlags.Below)
        {
            isJumping = false;
            yVelocity = 0;
        }

        if(Input.GetButtonDown("Jump")&& !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
        }
        
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        cc.Move(dir * moveSpeed * Time.deltaTime);
        hpSlider.value = (float)hp / (float)maxHp;
    }
    public void DamageAction(int damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            hp = 0;
        }
        else
        {
            StartCoroutine(PlayHitEffect());
        }
    }
    IEnumerator PlayHitEffect()
    {
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitEffect.SetActive(false);
    }
}
