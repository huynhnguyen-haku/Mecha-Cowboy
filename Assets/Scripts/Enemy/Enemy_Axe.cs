using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;

    private Vector3 direction;
    private Transform player;
    private float flySpeed;
    private float rotationSpeed;
    private float timer = 1;           // Time the axe will home toward the player
    private float currentLifeTime = 10;
    private int damage;

    // Initialize axe parameters and target
    public void AxeSetup(float flySpeed, Transform player, float timer, int damage)
    {
        rotationSpeed = 1600;
        this.damage = damage;
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        currentLifeTime -= Time.deltaTime;

        UpdateRotation();

        // Home toward player while timer is active
        if (timer > 0)
            UpdateDirection();

        // Return to pool if lifetime expires
        if (currentLifeTime <= 0)
            ReturnAxeToPool();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction.normalized * flySpeed;
    }

    // Rotate axe visual and align with velocity
    private void UpdateRotation()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        transform.forward = rb.linearVelocity;
    }

    // Update direction toward player
    private void UpdateDirection()
    {
        direction = player.position + Vector3.up - transform.position;
    }

    // Return axe to object pool
    private void ReturnAxeToPool()
    {
        ObjectPool.instance.ReturnObject(gameObject);
    }

    // Handle collision: deal damage, play FX, and return to pool
    private void OnCollisionEnter(Collision collision)
    {
        I_Damagable damagable = collision.gameObject.GetComponent<I_Damagable>();
        damagable?.TakeDamage(damage);

        GameObject newFx = ObjectPool.instance.GetObject(impactFx, transform);

        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFx, 1f);
    }
}
