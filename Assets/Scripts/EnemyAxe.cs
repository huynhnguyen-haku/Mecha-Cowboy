using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactFX;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;
    private Transform player;

    private float flySpeed;
    private float rotateSpeed;
    private Vector3 direction;

    private float timer = 1;


    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
        rotateSpeed = 1600;

        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotateSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer > 0)
        {
            direction = player.position + Vector3.up - transform.position;
        }

        rb.linearVelocity = direction.normalized * flySpeed;
        transform.forward = rb.linearVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if (bullet != null || player != null)
        {
            GameObject newImpactFX = ObjectPool.instance.GetObject(impactFX);
            newImpactFX.transform.position = transform.position;

            ObjectPool.instance.ReturnObject(gameObject);
            ObjectPool.instance.ReturnObject(newImpactFX, 1);
        }
    }
}

