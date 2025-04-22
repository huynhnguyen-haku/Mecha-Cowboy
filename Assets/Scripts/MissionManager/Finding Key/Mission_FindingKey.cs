using UnityEngine;

[CreateAssetMenu(fileName = "New Key Find Mission", menuName = "Mission/Key Find Mission")]
public class Misson_KeyFind : Mission
{
    [SerializeField] private GameObject key;
    private bool isKeyFound;
    public override void StartMission()
    {
        MissionObject_Key.OnKeyPickedUp += PickupKey;

        UI.instance.inGameUI.UpdateMissionUI("Find the Key-holder and retrive the key.");

        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key);
        enemy.MakeEnemyStronger();
    }

    public override bool MissionCompleted()
    {
        return isKeyFound;
    }


    private void PickupKey()
    {
        isKeyFound = true;
        MissionObject_Key.OnKeyPickedUp -= PickupKey;
        
        UI.instance.inGameUI.UpdateMissionUI("Key Found! \n Now go to the airplane to escape.");
    }
}
