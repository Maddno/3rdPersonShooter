using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControler : MonoBehaviour
{
    //[SerializeField] Slider helthSlider;
    [SerializeField] TextMeshProUGUI playerHealth;
    [SerializeField] TextMeshProUGUI amo;
    [SerializeField] GameObject[] weaponIndication = new GameObject[3];

    private void Start() 
    {
        for (int indexWeap = 0; indexWeap < weaponIndication.Length; indexWeap++)
        {
            weaponIndication[indexWeap].SetActive(false); 
        }
    }
    public void SetHealth (string i) { playerHealth.text = i; }
    public void SetAmo (string i) { amo.text = i; }
    public void SetWeaponToDisplay (int i)
    {
        for (int indexWeap = 0; indexWeap < weaponIndication.Length; indexWeap++)
        {
            weaponIndication[indexWeap].SetActive(false); 
        }
        for (int indexWeap = 0; indexWeap < weaponIndication.Length; indexWeap++)
        {
            if (i == indexWeap) { weaponIndication[i].SetActive(true); }
        }
    }
    
}
