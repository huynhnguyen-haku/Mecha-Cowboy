using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private BoxCollider boxCollider;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            enemy.GetHit();
        }

        CreateImpactFX(collision);
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool()
      =>  ObjectPool.instance.ReturnObject(gameObject);
    

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

