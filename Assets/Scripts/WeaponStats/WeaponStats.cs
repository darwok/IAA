using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponStats", menuName = "Game/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    [Header("Core Stats")]
    public float damage = 10f;
    public float fireRate = 0.5f; // Tiempo entre disparos en segundos

    [Header("Ammo & Magazine")]
    public bool infiniteAmmo = true;
    public bool infiniteMagazine = false;
    public bool requiresReload = true;
    public int maxMagazineSize = 30;
    public int maxTotalAmmo = 90;
    public float reloadTime = 1.5f;

    [Header("Prefabs")]
    public GameObject projectilePrefab;
    public GameObject hitFeedbackPrefab;
}