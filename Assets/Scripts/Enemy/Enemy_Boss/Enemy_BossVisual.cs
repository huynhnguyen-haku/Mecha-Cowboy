using UnityEngine;

public class Enemy_BossVisual : MonoBehaviour
{
    private Enemy_Boss enemy;
    [SerializeField] private GameObject[] batteries;
    [SerializeField] private float initialBatteryCharge = 0.2f;

    private float dischargeSpeed;
    private float rechargeSpeed;

    private bool isRecharging;


    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();
        ResetBatteries();
    }

    private void Update()
    {
        UpdateBatteriesScale();
    }

    private void UpdateBatteriesScale()
    {
        if (batteries.Length <= 0)
            return;

        foreach (GameObject battery in batteries)
        {
            if (battery.activeSelf)
            {
                float scaleChange = (isRecharging ? rechargeSpeed : -dischargeSpeed) * Time.deltaTime;
                float newScaleY = Mathf.Clamp(battery.transform.localScale.y + scaleChange, 0, initialBatteryCharge);

                battery.transform.localScale = new Vector3(0.15f, newScaleY, 0.15f);

                if (battery.transform.localScale.y <= 0)
                {
                    battery.SetActive(false); // Deactivate the battery when it is empty, so no error will occur
                }
            }
        }
    }

    public void ResetBatteries()
    {
        isRecharging = true;
        rechargeSpeed = initialBatteryCharge / enemy.abilityCooldown;
        dischargeSpeed = initialBatteryCharge / (enemy.flamethrowDuration * 0.75f);

        foreach (GameObject battery in batteries)
        {
            battery.SetActive(true);
        }
    }

    public void DischargeBatteries()
    {
        isRecharging = false;
    }
}
