using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk { None, RunToCover, ChangeCover}
public class Enemy_Range : Enemy
{
    [Header("Enemy Perks")]
    public CoverPerk coverPerk;

    [Header("Advance Perks")]
    public float advanceSpeed;
    public float advanceStoppingDistance;

    [Header("Cover System")]
    public float safeDistance;
    public CoverPoint lastCover { get; private set; }
    public CoverPoint currentCover { get; private set; }


    [Header("Weapon Details")]
    public Enemy_RangeWeaponType weaponType;
    public Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;


    [SerializeField] List<Enemy_RangeWeaponData> avaiableWeaponData;

    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_Range runToCoverState { get; private set; }
    public AdvancePlayerState_Range advancePlayerState { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        enemyVisual.SetupVisual();
        SetupWeapon();
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public void FireSingleBullet()
    {
        anim.SetTrigger("Fire");
        Vector3 bulletsDirection = ((player.position + Vector3.up) - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<Enemy_Bullet>().BulletSetup();
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
        gunPoint = enemyVisual.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;

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

        Debug.Log("No cover found");
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
}
