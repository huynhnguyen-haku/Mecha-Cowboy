using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolConfig
{
    public GameObject prefab;
    public int poolSize = 10;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private List<PoolConfig> pools;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private Dictionary<GameObject, int> poolSizeDictionary = new();


    #region Unity Methods

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var pool in pools)
        {
            InitializeNewPool(pool.prefab, pool.poolSize);
        }
    }

    #endregion

    #region Pool Access

    // Get an object from the pool, or create if needed
    public GameObject GetObject(GameObject prefab, Transform target)
    {
        if (!poolDictionary.ContainsKey(prefab))
            InitializeNewPool(prefab, poolSizeDictionary.ContainsKey(prefab) ? poolSizeDictionary[prefab] : 10);

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        objectToGet.transform.position = target.position;
        objectToGet.transform.parent = null;
        objectToGet.SetActive(true);

        return objectToGet;
    }

    // Return an object to the pool after a delay
    public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn));
    }

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objectToReturn);
    }

    // Deactivate and enqueue the object
    private void ReturnToPool(GameObject objectToReturn)
    {
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = transform;

        poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    #endregion

    #region Pool Initialization

    // Create a new pool for a prefab
    private void InitializeNewPool(GameObject prefab, int size)
    {
        poolDictionary[prefab] = new Queue<GameObject>();
        poolSizeDictionary[prefab] = size;
        for (int i = 0; i < size; i++)
        {
            CreateNewObject(prefab);
        }
    }

    // Instantiate and add a new object to the pool
    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;
        newObject.SetActive(false);
        poolDictionary[prefab].Enqueue(newObject);
    }

    #endregion
}
