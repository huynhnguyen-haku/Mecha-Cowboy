using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        Destroy(gameObject); // Destroy the bullet on collision
    }

    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

            Destroy(newImpactFX, 1);
        }
    }
}

