using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private Transform firePoint;

    private int currentMagazineAmmo;
    private int currentTotalAmmo;
    private float nextFireTime;
    private bool isReloading;

    private void Start()
    {
        currentMagazineAmmo = weaponStats.maxMagazineSize;
        currentTotalAmmo = weaponStats.maxTotalAmmo;
    }

    public void TryShoot()
    {
        if (isReloading || Time.time < nextFireTime) return;

        if (currentMagazineAmmo > 0 || weaponStats.infiniteMagazine)
        {
            Shoot();
        }
        else if (weaponStats.requiresReload && (currentTotalAmmo > 0 || weaponStats.infiniteAmmo))
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private void Shoot()
    {
        nextFireTime = Time.time + weaponStats.fireRate;

        if (!weaponStats.infiniteMagazine)
        {
            currentMagazineAmmo--;
        }

        // Instanciar proyectil y pasarle el daño y el prefab de impacto
        GameObject projObj = Instantiate(weaponStats.projectilePrefab, firePoint.position, firePoint.rotation);
        if (projObj.TryGetComponent(out Projectile projectile))
        {
            projectile.Initialize(weaponStats.damage, weaponStats.hitFeedbackPrefab);
        }
    }

    public void TriggerManualReload()
    {
        if (!isReloading && currentMagazineAmmo < weaponStats.maxMagazineSize && (currentTotalAmmo > 0 || weaponStats.infiniteAmmo))
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponStats.reloadTime);

        if (weaponStats.infiniteAmmo)
        {
            currentMagazineAmmo = weaponStats.maxMagazineSize;
        }
        else
        {
            int bulletsNeeded = weaponStats.maxMagazineSize - currentMagazineAmmo;
            int bulletsToLoad = Mathf.Min(bulletsNeeded, currentTotalAmmo);

            currentMagazineAmmo += bulletsToLoad;
            currentTotalAmmo -= bulletsToLoad;
        }

        isReloading = false;
    }
}