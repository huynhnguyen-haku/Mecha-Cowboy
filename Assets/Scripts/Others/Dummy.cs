using UnityEngine;

public class Dummy : MonoBehaviour, I_Damagable
{
    public int currentHealth;
    public int maxHealth = 300;

    [Space]

    public MeshRenderer mesh;
    public Material white;
    public Material red;

    [Space]

    public float refreshCooldown;
    private float lastTimeDamaged;

    private void Start()
    {
        Fresh();
    }

    private void Update()
    {
        if (Time.time > lastTimeDamaged + refreshCooldown || Input.GetKeyDown(KeyCode.B))
        {
            Fresh();
        }
    }

    private void Fresh()
    {
        currentHealth = maxHealth;
        mesh.sharedMaterial = white;
        Debug.Log("Dummy is ready to go!");
    }

    public void TakeDamage(int damage)
    {
        lastTimeDamaged = Time.time;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Dummy has died.");
        mesh.sharedMaterial = red;
    }
}
