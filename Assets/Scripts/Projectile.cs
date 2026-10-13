using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;

    private float damage;
    private GameObject hitVFXPrefab;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(float damageAmt, GameObject vfxPrefab)
    {
        damage = damageAmt;
        hitVFXPrefab = vfxPrefab;
        rb.linearVelocity = transform.forward * speed; // En Unity 6 se usa linearVelocity
    }

    private void OnTriggerEnter(Collider other)
    {
        // Aplicar daño
        if (other.TryGetComponent(out Health healthComponent))
        {
            healthComponent.TakeDamage(damage);
        }

        // Generar Feedback Visual
        if (hitVFXPrefab != null)
        {
            // Usamos ClosestPoint para que la partícula aparezca en el borde del impacto
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Instantiate(hitVFXPrefab, hitPoint, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}