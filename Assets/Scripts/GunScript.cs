using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    public UIControler uIControler;

    [SerializeField] private float gunDamage = 10f;
    [SerializeField] private float gunRange = 100f;
    [SerializeField] private float fireRate = 15f;

    [SerializeField] public int maxAmo = 10;
    public int currentAmmo;
    [SerializeField] private float reloadTime = 2f; 
    private bool isReloading = false;
    
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    private PlayerInput playerInput;
    
    private float nextTimeToFire = 0f;

    private InputAction shootingAction;
    
    private void Awake() 
    {
        playerInput = GetComponent<PlayerInput>();
        shootingAction = playerInput.actions["Shoot"];

        currentAmmo = maxAmo;
    }

    private void OnEnable()
    {
        isReloading = false;
    }

    private void Start() 
    {
        uIControler = GameObject.FindGameObjectWithTag("UISystem").GetComponent<UIControler>();

        uIControler.SetAmo(currentAmmo + " / " + maxAmo.ToString());
    }

    private void Update()
    {
        if (isReloading)
            return;

        if ( currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(shootingAction.IsPressed() && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
        uIControler.SetAmo(currentAmmo + " / " + maxAmo.ToString());
    }

    public float GetDamage()
    {
        return gunDamage;
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmo;

        uIControler.SetAmo(currentAmmo + " / " + maxAmo.ToString());
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        currentAmmo--;

        uIControler.SetAmo(currentAmmo + " / " + maxAmo.ToString());

        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, gunRange))
        {
            Debug.Log(hit.transform.name);
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(gunDamage);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
