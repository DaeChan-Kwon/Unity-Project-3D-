using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    // 아이템 소지 여부
    public bool hasOil = false;
    public bool hasKey = false;

    // UI 아이콘
    public GameObject oilIcon;
    public GameObject keyIcon;

    void Start()
    {
        UpdateUI();
    }

    public void PickupOil()
    {
        hasOil = true;
        UpdateUI();
    }

    public void UseOil()
    {
        hasOil = false;
        UpdateUI();
    }

    public void PickupKey()
    {
        hasKey = true;
        UpdateUI();
    }

    // UI 아이콘 업데이트
    void UpdateUI()
    {
        if (oilIcon != null) oilIcon.SetActive(hasOil);
        if (keyIcon != null) keyIcon.SetActive(hasKey);
    }
}