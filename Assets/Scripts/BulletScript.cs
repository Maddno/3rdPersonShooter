using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] public float bulletDamage = 10f;


    private void OnTriggerEnter(Collider other) 
    {
        
        if(other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
    
}
