using UnityEngine;

[CreateAssetMenu(fileName = "New Key Find Mission", menuName = "Mission/Key Find Mission")]
public class Misson_KeyFind : Mission
{
    [SerializeField] private GameObject key;
    private bool keyFound;
    public override void StartMission()
    {
        MissionObject_Key.OnKeyPickedUp += PickupKey;
        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key);
    }

    public override bool MissionCompleted()
    {
        return keyFound;
    }


    private void PickupKey()
    {
        keyFound = true;
        MissionObject_Key.OnKeyPickedUp -= PickupKey;
        Debug.Log("Key Found");
    }
}
