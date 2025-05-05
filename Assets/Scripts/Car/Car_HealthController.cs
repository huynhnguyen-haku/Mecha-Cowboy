using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, I_Damagable
{
    private Car_Controller carController;

    public int maxHealth;
    public int currentHealth;

    private bool carBroken;

    [Header("Explosion Setting")]
    [SerializeField] private int explosionDamage = 350;
    [SerializeField] private float explosionRadius = 5;   
    [Space]
    [SerializeField] private ParticleSystem fireFX;
    [SerializeField] private ParticleSystem explosionFX;
    [SerializeField] private Transform explosionPoint;
    [Space]
    [SerializeField] private float explosionDelay = 3;
    [SerializeField] private float explosionForce = 7;
    [SerializeField] private float explosionUpwardModifier = 2;


    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (fireFX.gameObject.activeSelf)
        {
            fireFX.transform.rotation = Quaternion.identity;
        }
    }

    public void UpdateCarHealthUI()
    {
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth, maxHealth);
    }


    private void ReduceHealth(int damage)
    {
        if (carBroken)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            BreakTheCar();
        }

    }

    private void BreakTheCar()
    {
        carBroken = true;
        carController.BreakCar();

        fireFX.gameObject.SetActive(true);
        StartCoroutine(ExplosionCar(explosionDelay));
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage);
        UpdateCarHealthUI();
    }

    private IEnumerator ExplosionCar(float delay)
    {
        yield return new WaitForSeconds(delay);

        explosionFX.gameObject.SetActive(true);
        carController.rb.AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardModifier, ForceMode.Impulse);

        Explode();
    }

    private void Explode()
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(explosionPoint.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            I_Damagable damagable = hit.GetComponent<I_Damagable>();
            if (damagable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;
                if (uniqueEntities.Add(rootEntity) == false)
                    continue; // Skip if the entity has already been hit

                damagable.TakeDamage(explosionDamage);

                hit.GetComponentInChildren<Rigidbody>().
                    AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardModifier, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(explosionPoint.position, explosionRadius);
    }
}
