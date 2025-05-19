using UnityEngine;

public class Enemy_BossVisual : MonoBehaviour
{
    private Enemy_Boss enemy;

    [Header("Jump Attack Visual")]
    [SerializeField] private float landingOffset = 1; // Offset for landing zone placement
    [SerializeField] private ParticleSystem landingZoneFX; // Particle effect for landing zone
    [SerializeField] private GameObject[] weaponTrails; // Array of weapon trail objects

    [Header("Flamethrower Battery")]
    [SerializeField] private GameObject[] batteries; // Array of battery game objects
    [SerializeField] private float initialBatteryCharge = 0.2f; // Initial scale of batteries

    [Space]

    private float dischargeSpeed; // Speed at which batteries discharge
    private float rechargeSpeed; // Speed at which batteries recharge
    private bool isRecharging; // Tracks if batteries are recharging

    #region Unity Methods
    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();

        landingZoneFX.transform.parent = null;
        landingZoneFX.Stop();

        ResetBatteries();
    }

    private void Update()
    {
        UpdateBatteriesScale();
    }
    #endregion

    #region Visual Effects Methods
    // Enable the weapon trail effect
    public void EnableWeaponTrail(bool active)
    {
        if (weaponTrails.Length <= 0)
            return;

        foreach (var trail in weaponTrails)
        {
            if (trail != null)
                trail.SetActive(active);
        }
    }

    // Set up the landing zone effect for the boss jump attack
    public void PlaceLandingZone(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 offset = direction.normalized * landingOffset;

        landingZoneFX.transform.position = target + offset;
        landingZoneFX.Clear();

        var mainModule = landingZoneFX.main;
        mainModule.startLifetime = enemy.travelTimeToTarget * 2;

        landingZoneFX.Play();
    }
    #endregion

    #region Battery Management
    // Update the scale of the batteries based on their charge state
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
                    battery.SetActive(false); // Deactivate the battery when it is empty
            }
        }
    }

    // Reset the batteries to their initial state
    public void ResetBatteries()
    {
        isRecharging = true;
        rechargeSpeed = initialBatteryCharge / enemy.abilityCooldown;
        dischargeSpeed = initialBatteryCharge / (enemy.flamethrowDuration * 0.75f);

        foreach (GameObject battery in batteries)
            battery.SetActive(true);
    }

    // Start discharging the batteries when the flamethrower is activated
    public void DischargeBatteries()
    {
        isRecharging = false;
    }
    #endregion
}