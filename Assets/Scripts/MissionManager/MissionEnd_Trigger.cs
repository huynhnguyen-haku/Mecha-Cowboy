using UnityEngine;

public class MissionEnd_Trigger : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player)
            return;

        // Kiểm tra nếu mission hiện tại là Mission_Timer và đánh dấu hoàn thành
        Mission_Timer timerMission = MissionManager.instance.currentMission as Mission_Timer;
        if (timerMission != null)
        {
            timerMission.MarkAsCompleted();
        }

        if (MissionManager.instance.MissionCompleted())
        {
            GameManager.instance.CompleteGame();
            Debug.Log("Level completed!");

            // Reset MissionManager để ngăn lỗi sau RestartScene
            MissionManager.instance.ResetAfterCompletion();
        }
    }
}