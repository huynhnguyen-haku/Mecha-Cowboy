using UnityEngine;

public class Enemy_Bullet : Bullet
{
    protected override void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        ReturnBulletToPool();

        Player player = collision.gameObject.GetComponentInParent<Player>();
    }
}
