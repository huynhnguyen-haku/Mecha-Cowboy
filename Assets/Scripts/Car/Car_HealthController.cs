using System;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, I_Damagable
{
    private Car_Controller carController;

    public int maxHealth;
    public int currentHealth;

    private bool carBroken;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        currentHealth = maxHealth;
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
        // Add logic to handle car breakage (e.g., play animation, disable controls, etc.)
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage);
        UpdateCarHealthUI();
    }
}
