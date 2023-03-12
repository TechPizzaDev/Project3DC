using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pistol : MonoBehaviour
{

    //REMEMBER, THE FIRST PERSON CONTROLLER HAS A CAMERA MOVEMENT THRESHOLD.
    //REMOVE IT!!!

    //bullet
    [SerializeField]
    private GameObject projectilePrefab;
    
    //force
    [SerializeField]
    private float projectileForce, upwardForce;

    //weapon stats
    [SerializeField]
    private float timeBetweenShooting, spread, reloadTime, timeBetweenShots, rpm;
    [SerializeField]
    private int magSize, bulletsPerClick;
    [SerializeField]
    private bool holdToFire;

    [SerializeField]
    private int bulletsLeft, bulletsShot;
    
    //bools
    [SerializeField]
    private bool shooting, readyToShoot, reloading;
    
    //references
    [SerializeField]
    private Camera fpsCam;
    [SerializeField]
    private Transform attackPoint;

    //debug
    [SerializeField]
    private bool allowInvoke = true;

    public float Rpm
    {
        get { return rpm; }
        set
        {
            rpm = value;
            timeBetweenShooting = 60 / rpm ;
        }
    }

    private void Awake()
    {
        if (rpm != 0)
        {
            timeBetweenShooting = 60 / rpm;
        }
        bulletsLeft = magSize;
        readyToShoot = true;
    }

    private void Update()
    {
        if (holdToFire && shooting && readyToShoot && !reloading && bulletsLeft > 0)
        {
            Fire();
        }
    }

    public void OnReload(InputAction.CallbackContext value)
    {
        if (value.started && bulletsLeft < magSize && !reloading)
        {
            Reload();
        }
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (holdToFire)
        {
            if (value.started)
            {
                shooting = true;
            }
            else if (value.canceled)
            {
                shooting = false;
            }
        }
        else
        {
            //if (value.started)
            //{
            //    shooting = value.started;

            //    //shooting = true;
            //}

            //might need to be like this
            if (value.started && readyToShoot && !reloading && bulletsLeft > 0)
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        readyToShoot= false;

        //Find hit position with raycast 
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //ray through middle of screen
        RaycastHit hit;

        //Check for hit
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100); //Point far away in case of shooting air
        }

        //Direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //Instantiate projectile
        GameObject currentProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        //Rotate bullet
        currentProjectile.transform.forward = directionWithSpread.normalized;

        //Add forces to projectile
        Rigidbody projectileRb = currentProjectile.GetComponent<Rigidbody>();
        projectileRb.AddForce(directionWithSpread.normalized * projectileForce, ForceMode.Impulse);
        projectileRb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function with timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //if more than one bulletPerClick repeat shoot function
        if (bulletsShot < bulletsPerClick && bulletsLeft < 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        //allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magSize;
        reloading = false;
    }
}
