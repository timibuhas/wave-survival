using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon:MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;

    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("Reload")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;


    bool isADS;
    public enum WeaponModel
    {
        UZI,
        AK47,
        M1911
    }
    public WeaponModel thisWeaponModel;
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto

    }
    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        spreadIntensity = hipSpreadIntensity;
    }

    void Update()
    {
        if (isActiveWeapon)
        {

            gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");

            }

            if (Input.GetMouseButtonDown(1))
            {
                animator.SetTrigger("enterADS");
                isADS = true;
                HUDManager.Instance.middleDot.SetActive(false);
                spreadIntensity = adsSpreadIntensity;
            }
            if (Input.GetMouseButtonUp(1))
            {
                animator.SetTrigger("exitADS");
                isADS = false;
                HUDManager.Instance.middleDot.SetActive(true);
                spreadIntensity = hipSpreadIntensity;
            }
            GetComponent<Outline>().enabled = false;
            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSoundUZI.Play();
            }
            if (currentShootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel)>0)
            {
                Reload();
            }
            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0 && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel)>0)
            {
                Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");

            }
            
        }
     
        
    }



    private void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        if (isADS)
        {
            animator.ResetTrigger("RECOIL_ADS");
            animator.SetTrigger("RECOIL_ADS");
            
        }
        else
        {
            animator.ResetTrigger("RECOIL");
            animator.SetTrigger("RECOIL");
        }
        
        // SoundManager.Instance.shootingSoundUZI.Play();
        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet,bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if(currentShootingMode==ShootingMode.Burst && burstBulletsLeft>1 && bulletsLeft>0)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        animator.SetTrigger("RELOAD");
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }
    private void ReloadCompleted()
    {
        int bulletsNeeded = magazineSize - bulletsLeft;
        int availableAmmo = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        int bulletsToReload = Math.Min(bulletsNeeded, availableAmmo);

        bulletsLeft += bulletsToReload;

        WeaponManager.Instance.DecreaseTotalAmmo(bulletsToReload, thisWeaponModel);

        isReloading = false;
    }
    private void ResetShot()
    {
        allowReset = true;
        readyToShoot = true;
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray,out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(0, y, x);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
