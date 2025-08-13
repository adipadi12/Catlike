using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunProjectile : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    
    [SerializeField] float shootForce, upwardForce;

    [SerializeField] private float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    [SerializeField] private int magazineSize, bulletsPerTap;
    [SerializeField] private bool allowButtonHold;

    private int bulletsLeft, bulletsShot;

    private bool shooting, readyToShoot, reloading;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject muzzleFlash;
    
    public bool allowInvoke = true;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    void MYInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //reload
        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        //reload automatically
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();
        
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Update()
    {
        MYInput();
    }

    void Shoot()
    {
        readyToShoot = false;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
        {
            targetPoint = ray.GetPoint(100); //point far away from the player
        }

        Vector3 directionWithoutSpread =  attackPoint.transform.forward;

        // //spread
        // float x =  Random.Range(-spread, spread);
        // float y =  Random.Range(-spread, spread);
        //
        // // direcn with spread
        Vector3 directionWithSpread = directionWithoutSpread;
        
        //instantiate
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.GetComponent<MeshRenderer>().material
            .SetColor("_BaseColor", Color.Lerp(Color.red, Color.black, 3));
        Debug.Log("Bullet shot");
        
        currentBullet.transform.forward = directionWithSpread.normalized; //rotate bullet to shoot direction
        
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }
        
        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
    }

    void ResetShot()
    {
        readyToShoot =  true;
        allowInvoke = true;
    }

    void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
    }
}
