using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardsMulti = 1;
    private Rigidbody rb;
    private float timer;
    private float impactPower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        GameObject newFX = ObjectPool.instance.GetObject(explosionFX, transform);

        ObjectPool.instance.ReturnObject(gameObject); // Return the grenade
        ObjectPool.instance.ReturnObject(newFX, 1); // Return the explosion fx atfer 1s

        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMulti, ForceMode.Impulse);
            }

        }
    }

    public void SetupGrenade(Vector3 target, float timeToTarget, float countdown, float impactPower)
    {
        rb.linearVelocity = CalculateLaunchVelocity(target, timeToTarget);
        timer = countdown + timeToTarget;
        this.impactPower = impactPower;
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

