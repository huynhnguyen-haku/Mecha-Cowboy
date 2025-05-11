using UnityEngine;

public class Car_DamageZone : MonoBehaviour
{
    private Car_Controller carController;
    [SerializeField] private float minSpeedToDamage = 4f;

    [SerializeField] private int carDamage;
    [SerializeField] private float impactForce = 150;
    [SerializeField] private float upwardsMulti = 3;

    private void Awake()
    {
        carController = GetComponentInParent<Car_Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // The car is not moving fast enough to cause damage.
        if (carController.speed < minSpeedToDamage)
            return;

        I_Damagable damagable = other.GetComponent<I_Damagable>();
        if (damagable == null)
            return;

        damagable.TakeDamage(carDamage);

        // If the enemy has a rigidbody, then apply force to it.
        Rigidbody rigidbody = other.GetComponent<Rigidbody>();
        if (rigidbody != null)
            ApplyForce(rigidbody);

    }

    private void ApplyForce(Rigidbody rigidbody)
    {
        if (rigidbody == null)
            return;

        rigidbody.isKinematic = false;
        rigidbody.AddExplosionForce(impactForce, transform.position, 3, upwardsMulti, ForceMode.Impulse);
    }
}
