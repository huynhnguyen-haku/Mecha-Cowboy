using UnityEngine;

public class Enemy_DropController : MonoBehaviour
{
    [SerializeField] private GameObject missionObjectKey;

    public void GiveKey(GameObject newKey)
    {
        missionObjectKey = newKey;
    }

    public void DropItems()
    {
        if (missionObjectKey != null)
        {
            CreateItem(missionObjectKey);
        }
    }

    private void CreateItem(GameObject itemPrefab)
    {
        GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
    }
}
