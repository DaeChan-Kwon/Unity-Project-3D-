using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public float rotateSpeed = 50f;

    void Update()
    {
        // 아이템 회전
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.PickupKey();
                Destroy(gameObject);
            }
        }
    }
}