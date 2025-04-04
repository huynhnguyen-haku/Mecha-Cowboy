using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private BoxCollider boxCollider;
    private Rigidbody rb;

    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private float bulletLifeTime;
    private float currentLifeTime;

    private float impactForce;

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

    public void BulletSetup(float impactForce = 100)
    {
        boxCollider.enabled = true;
        this.impactForce = impactForce;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        ReturnBulletToPool();

        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        Enemy_Shield shield = collision.gameObject.GetComponent<Enemy_Shield>();

        if (shield != null)
        {
            shield.ReduceDurability();
            return;
        }

        if (enemy != null)
        {
            Vector3 force = rb.linearVelocity.normalized * impactForce;
            Rigidbody hitRigibBody = collision.collider.attachedRigidbody;

            enemy.GetHit();
            enemy.DeathImpact(force, collision.contacts[0].point, hitRigibBody);
        }
    }

    protected void ReturnBulletToPool()
    {
        bulletTrail.Clear();
        ObjectPool.instance.ReturnObject(gameObject);
    }

    protected void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFX.transform.position = contact.point;

            ObjectPool.instance.ReturnObject(newImpactFX, 1);
        }
    }
}

