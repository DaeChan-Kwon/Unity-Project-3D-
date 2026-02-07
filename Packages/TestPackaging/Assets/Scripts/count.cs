using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class count : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public int countNum;

    void Start()
    {
        countNum = 0;
        textUI.text = "Count : " + countNum.ToString();
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            countNum++;
            Debug.Log("Å° ÀÔ·Â");
            textUI.text = "Count : "+ countNum.ToString();
        }
    }
}
