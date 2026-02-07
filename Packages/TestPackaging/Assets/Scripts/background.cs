using UnityEngine;

public class background : MonoBehaviour
{
    public Material bgMaterial;

    public float scrollSpeed = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = Vector2.up;
        bgMaterial.mainTextureOffset += dir * scrollSpeed * Time.deltaTime;
    }
}
