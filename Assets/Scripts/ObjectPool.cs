using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> bulletPool;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
            Destroy(gameObject);
    }

    private void Start()
    {
        bulletPool = new Queue<GameObject>();
        CreateInitialPool();
    }

    private void CreateInitialPool()
    {
        CreateNewBullet();
    }

    private void CreateNewBullet()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
        bullet.transform.SetParent(transform);
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
        {
            CreateNewBullet();
        }
        GameObject bulletToGet = bulletPool.Dequeue();
        bulletToGet.SetActive(true);
        bulletToGet.transform.SetParent(null);

        return bulletToGet;
    }
}