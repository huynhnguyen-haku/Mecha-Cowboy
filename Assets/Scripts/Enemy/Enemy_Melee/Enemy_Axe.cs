using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFX;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;

    private Vector3 direction;
    private Transform player;
    private float flySpeed;
    private float rotateSpeed;

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

        if (gameObject.layer != LayerMask.NameToLayer("Enemy_Axe"))
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy_Axe");
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        I_Damagable damagable = other.gameObject.GetComponent<I_Damagable>();

        if (damagable != null)
        {
            GameObject newImpactFX = ObjectPool.instance.GetObject(impactFX, transform);

            ObjectPool.instance.ReturnObject(gameObject);
            ObjectPool.instance.ReturnObject(newImpactFX, 1);
        }
    }
}

