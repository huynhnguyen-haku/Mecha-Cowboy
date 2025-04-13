using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private float impactForce;
    private BoxCollider boxCollider;
    private Rigidbody rb;

    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private float bulletLifeTime;
    private float currentLifeTime;

    private LayerMask allyLayerMask;
    private bool canCollide; // Flag to check if the bullet has already collided

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        bulletTrail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        currentLifeTime = bulletLifeTime;
    }

    protected virtual void Update()
    {
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            ReturnBulletToPool();
        }
    }

    public void BulletSetup(LayerMask allyLayerMask, float impactForce = 100)
    {
        this.allyLayerMask = allyLayerMask;
        this.impactForce = impactForce;

        boxCollider.enabled = true;
        bulletTrail.Clear();
        canCollide = true; // Reset the flag when the bullet is setup
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) return; // If the bullet has already collided, do nothing

        canCollide = false; // Set the flag to false to prevent further collisions

        if (FriendlyFire() == false)
        {
            if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(10);
                return;
            }
        }

        CreateImpactFX();
        ReturnBulletToPool();

        I_Damagable damagable = collision.gameObject.GetComponentInChildren<I_Damagable>(); // Check for damageable component (even in children)
        damagable?.TakeDamage();

        Enemy_Shield shield = collision.gameObject.GetComponent<Enemy_Shield>();

        if (shield != null)
        {
            shield.ReduceDurability();
            return;
        }

        ApplyBulletImpact(collision);
    }

    private void ApplyBulletImpact(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.linearVelocity.normalized * impactForce;
            Rigidbody hitRigibBody = collision.collider.attachedRigidbody;
            enemy.BulletImpact(force, collision.contacts[0].point, hitRigibBody);
        }
    }

    protected void ReturnBulletToPool(float delay = 0)
    {
        ObjectPool.instance.ReturnObject(gameObject, delay);
    }

    protected void CreateImpactFX()
    {
        GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX, transform);
        ObjectPool.instance.ReturnObject(newImpactFX, 1);
    }

    public bool FriendlyFire()
    {
        return GameManager.instance.friendlyFire;
    }
}

