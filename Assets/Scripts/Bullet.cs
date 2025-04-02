using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private BoxCollider boxCollider;
    private Rigidbody rb;
    [SerializeField] private float bulletLifeTime;
    private float currentLifeTime;

    public float impactForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        currentLifeTime = bulletLifeTime;
    }

    private void Update()
    {
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            ReturnBulletToPool();
        }
    }

    public void BulletSetup(float impactForce)
    {
        boxCollider.enabled = true;
        this.impactForce = impactForce;
    }

    private void OnCollisionEnter(Collision collision)
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

    private void ReturnBulletToPool()
      => ObjectPool.instance.ReturnObject(gameObject);

    private void CreateImpactFX(Collision collision)
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

