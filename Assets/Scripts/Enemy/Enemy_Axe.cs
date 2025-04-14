using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;


    private Vector3 direction;
    private Transform player;
    private float flySpeed;
    private float rotationSpeed;
    private float timer = 1; // Time the axe will change direction towards the player
    private float currentLifeTime = 10;


    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
        rotationSpeed = 1600;

        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        currentLifeTime -= Time.deltaTime;

        UpdateRotation();

        if (timer > 0)
            UpdateDirection();

        if (currentLifeTime <= 0)
            ReturnAxeToPool();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction.normalized * flySpeed;
    }

    private void UpdateRotation()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        transform.forward = rb.linearVelocity;
    }

    private void UpdateDirection()
    {
        direction = player.position + Vector3.up - transform.position;
    }

    private void ReturnAxeToPool()
    {
        ObjectPool.instance.ReturnObject(gameObject);
    }




    private void OnCollisionEnter(Collision collision)
    {
        I_Damagable damagable = collision.gameObject.GetComponent<I_Damagable>();
        damagable?.TakeDamage();


        GameObject newFx = ObjectPool.instance.GetObject(impactFx, transform);

        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFx, 1f);
    }
}
