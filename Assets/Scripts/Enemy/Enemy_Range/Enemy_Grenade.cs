using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardsMulti = 1;
    private Rigidbody rb;
    private float timer;
    private float impactPower;
    private int grenadeDamage;

    private LayerMask allyLayerMask;
    private bool canExplode; // Flag to check if the grenade has already exploded  

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Ensure proper collision detection  
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0 && canExplode)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // No additional logic needed for collision with the ground
        // The grenade will naturally bounce and roll
    }


    private void Explode()
    {
        canExplode = false; // Set the flag to false to prevent further explosions  
        CreateExplosionFX();

        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>(); // To store unique entities hit by the explosion  
        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            I_Damagable damagable = hit.GetComponent<I_Damagable>();
            if (damagable != null)
            {
                if (IsTargetValid(hit) == false)
                    continue;

                GameObject rootEntity = hit.transform.root.gameObject;
                if (uniqueEntities.Add(rootEntity) == false)
                    continue; // Skip if the entity has already been hit  

                damagable.TakeDamage(grenadeDamage);
            }
            ApplyPhysicalForce(hit);
        }

        ObjectPool.instance.ReturnObject(gameObject); // Return the grenade to the pool
    }

    private void ApplyPhysicalForce(Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMulti, ForceMode.Impulse);
    }

    private void CreateExplosionFX()
    {
        GameObject newFX = ObjectPool.instance.GetObject(explosionFX, transform);

        ObjectPool.instance.ReturnObject(gameObject); // Return the grenade  
        ObjectPool.instance.ReturnObject(newFX, 2); // Return the explosion fx atfer 1s  
    }

    public void SetupGrenade(LayerMask allyLayerMask, Vector3 target, float timeToTarget, float countdown, float impactPower, int grenadeDamage)
    {
        this.allyLayerMask = allyLayerMask;
        this.grenadeDamage = grenadeDamage;
        this.impactPower = impactPower;

        timer = countdown + timeToTarget;
        canExplode = true; // Make grenade able to explode  

        rb.linearVelocity = CalculateLaunchVelocity(target, timeToTarget);
    }

    private bool IsTargetValid(Collider collider)
    {
        // Check if friendly fire is enabled, all colliders are valid  
        if (GameManager.instance.friendlyFire)
            return true;

        // If collider is on ally layer, target is invalid  
        if ((allyLayerMask & (1 << collider.gameObject.layer)) > 0)
            return false;

        return true;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);
        Vector3 velocityXZ = directionXZ / timeToTarget;

        float velocityY =
            (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;

        return launchVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}

