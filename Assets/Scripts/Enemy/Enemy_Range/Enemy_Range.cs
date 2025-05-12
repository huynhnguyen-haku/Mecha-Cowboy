using System.Collections.Generic;
using UnityEngine;


public enum CoverPerk { None, RunToCover, ChangeCover }
public enum UnstoppablePerk { None, Unstoppable }
public enum GrenadePerk { None, ThrowGrenade }
public class Enemy_Range : Enemy
{
    public Enemy_RangeSFX rangeSFX;

    [Header("Enemy Perks")]
    public Enemy_RangeWeaponType weaponType;
    public CoverPerk coverPerk;
    public UnstoppablePerk unstoppablePerk;
    public GrenadePerk grenadePerk;

    [Header("Advance Perks")]
    public float advanceSpeed;
    public float advanceStoppingDistance;
    public float advanceDuration;

    [Header("Grenade Perks")]
    public int grenadeDamage;
    public GameObject grenadePrefab;
    public float explosionTimer = 0.75f;
    public float timeToTarget = 1.2f;
    public float impactPower;

    public float grenadeCooldown;
    private float lastGrenadeThrowTime;
    [SerializeField] private Transform grenadeStartPoint;

    [Header("Cover System")]
    public float safeDistance;
    public CoverPoint lastCover { get; private set; }
    public CoverPoint currentCover { get; private set; }
    public float coverTime;


    [Header("Weapon Details")]
    public float attackDelay;
    public Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [Header("Aim Data")]
    public float slowAim = 4; // Use to slow down the reaction time of enemy after player is spotted
    public float fastAim = 20; // Use when enemy is in battle mode
    public Transform aim;
    public Transform playerBody;
    public LayerMask whatToIgnore;

    [Header("Minimap Icon")]
    private GameObject minimapIcon;


    [SerializeField] List<Enemy_RangeWeaponData> avaiableWeaponData;

    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_Range runToCoverState { get; private set; }
    public AdvancePlayerState_Range advancePlayerState { get; private set; }
    public ThrowGrenadeState_Range throwGrenadeState { get; private set; }
    public DeadState_Range deadState { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
        throwGrenadeState = new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
        deadState = new DeadState_Range(this, stateMachine, "Idle"); // Idle is used for placeholder

        rangeSFX = GetComponent<Enemy_RangeSFX>();

        // Add minimap icon
        if (minimapIcon == null)
        {
            var minimapSprite = GetComponentInChildren<MinimapSprite>(true);
            if (minimapSprite != null)
                minimapIcon = minimapSprite.gameObject;
        }
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        InitializePerk();
        visual.SetupVisual();

        SetupWeapon();
        aim.parent = null;
        playerBody = player.GetComponent<Player>().playerBody;
    }


    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public override void Die()
    {
        base.Die();

        if (!HealthController.muteDeathSound)
            rangeSFX.deadSFX.Play();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);

        if (minimapIcon != null)
            minimapIcon.SetActive(false);

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Enemy"));
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);

    }

    #region Weapon Setup

    public void FireSingleBullet()
    {
        anim.SetTrigger("Fire");

        // Play fire sfx
        var fireSFX = visual.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().fireSFX;
        if (fireSFX != null)
        {
            fireSFX.Play();
        }

        Vector3 bulletsDirection = (aim.position - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab, gunPoint);
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<Bullet>().BulletSetup(whatIsAlly, weaponData.bulletDamage);
        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.linearVelocity = bulletDirectionWithSpread * weaponData.bulletSpeed;
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        if (CanGetCover())
        {
            stateMachine.ChangeState(runToCoverState);
        }
        else
        {
            stateMachine.ChangeState(battleState);
        }
    }

    private void SetupWeapon()
    {
        List<Enemy_RangeWeaponData> filteredData = new List<Enemy_RangeWeaponData>();
        gunPoint = visual.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;

        foreach (var weaponData in avaiableWeaponData)
        {
            if (weaponData.weaponType == weaponType)
            {
                filteredData.Add(weaponData);
            }
        }

        if (filteredData.Count > 0)
        {
            weaponData = filteredData[Random.Range(0, filteredData.Count)];
        }
        else
        {
            Debug.LogError("No weapon data found for the specified weapon type.");
        }
    }

    #endregion

    #region Aim Setup

    public bool IsAimingOnPlayer()
    {
        float distanceAimToPlayer = Vector3.Distance(aim.position, player.position);

        return distanceAimToPlayer < 2;
    }

    public bool IsSeeingPlayer()
    {
        Vector3 myPosition = transform.position + Vector3.up;
        Vector3 directionToPlayer = playerBody.position - myPosition;

        if (Physics.Raycast(myPosition, directionToPlayer, out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
        {
            if (hit.transform.root == player.root)
            {
                UpdateAimPosition();
                return true;
            }
        }
        return false;
    }

    public void UpdateAimPosition()
    {
        float aimSpeed = IsAimingOnPlayer() ? fastAim : slowAim;
        aim.position = Vector3.MoveTowards(aim.position, playerBody.position, aimSpeed * Time.deltaTime);
    }

    #endregion

    #region Cover System

    private List<Cover> CollectNearByCovers()
    {
        float coverRadiusCheck = 30;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRadiusCheck);
        List<Cover> collectedCover = new List<Cover>();

        foreach (Collider collider in hitColliders)
        {
            Cover cover = collider.GetComponent<Cover>();

            if (cover != null && collectedCover.Contains(cover) == false)
                collectedCover.Add(cover);
        }
        return collectedCover;
    }

    public bool CanGetCover()
    {
        if (coverPerk == CoverPerk.None)
            return false;

        currentCover = AttempToFindCover()?.GetComponent<CoverPoint>();

        if (lastCover != currentCover && currentCover != null)
            return true;

        return false;

    }

    private Transform AttempToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();
        foreach (Cover cover in CollectNearByCovers())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        CoverPoint closestCoverPoint = null;
        float closestDistance = float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if (currentDistance < closestDistance)
            {
                closestCoverPoint = coverPoint;
                closestDistance = currentDistance;
            }
        }

        if (closestCoverPoint != null)
        {
            lastCover?.SetupOccupied(false);
            lastCover = currentCover;

            currentCover = closestCoverPoint;
            currentCover.SetupOccupied(true);

            return currentCover.transform;
        }
        return null;
    }
    #endregion

    public bool IsUnstoppable()
    {
        return unstoppablePerk == UnstoppablePerk.Unstoppable;
    }

    protected override void InitializePerk()
    {
        if (weaponType == Enemy_RangeWeaponType.Random)
        {
            ChooseRandomWeapon();
        }

        if (IsUnstoppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("AdvanceIndex", 1);
        }
    }

    private void ChooseRandomWeapon()
    {
        List<Enemy_RangeWeaponType> validTypes = new List<Enemy_RangeWeaponType>();
        foreach (Enemy_RangeWeaponType type in System.Enum.GetValues(typeof(Enemy_RangeWeaponType)))
        {
            if (type != Enemy_RangeWeaponType.Random && type != Enemy_RangeWeaponType.Sniper)
            {
                validTypes.Add(type);
            }
        }
        int RandomInxex = Random.Range(0, validTypes.Count);
        weaponType = validTypes[RandomInxex];
    }

    public bool CanThrowGrenade()
    {
        if (grenadePerk == GrenadePerk.None)
            return false;

        if (Vector3.Distance(player.transform.position, transform.position) < safeDistance)
            return false;

        if (Time.time > grenadeCooldown + lastGrenadeThrowTime)
            return true;

        return false;
    }

    public void ThrowGrenade()
    {
        lastGrenadeThrowTime = Time.time;
        visual.EnableGrenadeModel(false);

        GameObject newGrenade = ObjectPool.instance.GetObject(grenadePrefab, grenadeStartPoint);

        Enemy_Grenade newGrenadeScript = newGrenade.GetComponent<Enemy_Grenade>();

        if (stateMachine.currentState == deadState)
        {
            newGrenadeScript.SetupGrenade(whatIsAlly, transform.position, 1, explosionTimer, impactPower, grenadeDamage);
            return;
        }

        newGrenadeScript.SetupGrenade(whatIsAlly, player.transform.position, timeToTarget, explosionTimer, impactPower, grenadeDamage);
    }
}