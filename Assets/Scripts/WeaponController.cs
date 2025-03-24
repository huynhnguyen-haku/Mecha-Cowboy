using UnityEngine;

public class WeaponControls : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20;
    // This is the speed of the bullet from which our mass formula is derived

    private Player player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += context => Shot();
    }

    private void Shot()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();

        rbBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbBullet.linearVelocity = BullectDirection() * bulletSpeed;

        Destroy(bullet, 10); // This is a bullet life time, don't mistake it with destroy when colliding in Bullet.cs
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BullectDirection()
    {
        Transform aim = player.aim.Aim;

        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
            direction.y = 0;
        
        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim);

        return direction;
    }

    public Transform GunPoint => gunPoint;
}
