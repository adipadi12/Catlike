using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunProjectile : MonoBehaviour
{
    [SerializeField] private GameObject[] projectilePrefabs;
    
    [SerializeField] float shootForce, upwardForce;

    [SerializeField] private float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    [SerializeField] private int magazineSize, bulletsPerTap;
    [SerializeField] private bool allowButtonHold;

    private int bulletsLeft, bulletsShot;

    private bool shooting, readyToShoot, reloading;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform attackPoint;
    
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
        Vector3 directionWithSpread = directionWithoutSpread;
        
        //instantiate
        GameObject projectile = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];
        GameObject currentBullet = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Debug.Log("Bullet shot");
        StartCoroutine(DestroyAfterShot(currentBullet));
        
        currentBullet.transform.forward = directionWithSpread.normalized; //rotate projectilePrefabs to shoot direction
        
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        
        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
    }

    IEnumerator DestroyAfterShot(GameObject projectilePrefabs)
    {
        yield return new WaitForSeconds(5f);
        Destroy(projectilePrefabs);
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
