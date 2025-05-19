using System.Collections.Generic;
using UnityEngine;

public enum BossWeaponType { Flamethrower, Hammer }

public class Enemy_Boss : Enemy
{
    #region Fields and Properties
    [Header("Abilities")]
    public float minAbilityDistance; // Minimum distance required to use an ability
    public float abilityCooldown; // Cooldown time for abilities
    private float lastTimeAbility; // Last time an ability was used

    [Header("Attack")]
    [SerializeField] private int meleeAttackDamage; // Damage dealt by melee attacks
    [SerializeField] private Transform[] damagePoints; // Points to check for melee attack damage
    [SerializeField] private float attackCheckRadius; // Radius to check for attack hits
    [SerializeField] private GameObject meleeAttackFX; // Visual effect for melee attacks

    [Header("Boss Detail")]
    public BossWeaponType weaponType; // Type of weapon the boss uses
    public float attackRange; // Range within which the boss can attack
    public float actionCooldown = 10; // Cooldown for random actions
    public int attackAnimationCount; // Number of attack animations available

    [Header("Flamethrower")]
    public int flameDamage; // Damage dealt by flamethrower
    public float flameDamageCooldown; // Cooldown between flamethrower damage ticks
    public float flamethrowDuration; // Duration of the flamethrower activation
    public ParticleSystem flamethrower; // Particle system for flamethrower effect
    public bool flamethrowerActive { get; private set; } // Tracks if flamethrower is active

    [Header("Hammer")]
    public int hammerActiveDamage; // Damage dealt by hammer attack
    public GameObject activationPrefab; // Prefab for hammer activation effect
    [SerializeField] private float hammerCheckRadius; // Radius for hammer attack damage check

    [Header("Jump Attack")]
    public int jumpAttackDamage; // Damage dealt by jump attack
    public float impactRadius = 2.5f; // Radius of the jump attack impact
    public float impactPower = 10; // Force applied by jump attack impact
    public Transform impactPoint; // Point where the jump attack impacts
    public ParticleSystem jumpAttackVFX; // Visual effect for jump attack

    [Space]

    public float travelTimeToTarget = 1; // Time taken to reach the target during jump attack
    public float jumpAttackCooldown = 10; // Cooldown time for jump attack
    [SerializeField] private float upwardsMulti = 10; // Multiplier for upward force in impact
    private float lastTimeJump; // Last time a jump attack was performed
    public float minJumpDistanceRequired; // Minimum distance required for a jump attack

    [Header("Minimap Icon")]
    private GameObject minimapIcon; // Reference to the minimap icon

    [Space]

    [SerializeField] private LayerMask whatToIgnore; 

    public IdleState_Boss idleState { get; private set; } 
    public MoveState_Boss moveState { get; private set; } 
    public AttackState_Boss attackState { get; private set; } 
    public JumpAttackState_Boss jumpAttackState { get; private set; } 
    public AbilityState_Boss abilityState { get; private set; } 
    public DeadState_Boss deadState { get; private set; } 
    public Enemy_BossVisual bossVisual { get; private set; } 
    public Enemy_BossSFX bossSFX { get; private set; } 
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        bossVisual = GetComponent<Enemy_BossVisual>();
        bossSFX = GetComponent<Enemy_BossSFX>();

        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle");

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
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
            EnterBattleMode();

        MeleeAttackCheck(damagePoints, attackCheckRadius, meleeAttackFX, meleeAttackDamage);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.black;         // Black is melee attack range to activate
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.white;        // White is jump attack min range to activate
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        Gizmos.color = Color.green;        // Green is impact attack damage radius
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.blue;        // Blue is for the skill of the boss
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);

        Gizmos.color = Color.yellow;        // Yellow is for the attack hit check (if player stays inside, then it receives damage)

        if (damagePoints.Length > 0)
        {
            foreach (var damagePoint in damagePoints)
            {
                Gizmos.DrawWireSphere(damagePoint.position, attackCheckRadius);
            }

            // Red is for the hammer attack only
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(damagePoints[0].position, hammerCheckRadius);
        }
    }
    #endregion

    #region Battle Mode Methods
    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    // Checks if the player is within attack range (to perform melee attacks)
    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    // Checks if the player is in clear sight of the boss (to perform jump attacks/chase)
    public bool IsPlayerInClearSight()
    {
        Vector3 myPosition = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 playerPosition = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPosition - myPosition).normalized;

        if (Physics.Raycast(myPosition, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            if (hit.transform.root == player.root)
                return true;
        }
        return false;
    }
    #endregion

    #region Attack Methods
    // Activates or deactivates the flamethrower
    public void ActivateFlamethrower(bool activate)
    {
        flamethrowerActive = activate;

        if (!activate)
        {
            flamethrower.Stop();
            anim.SetTrigger("StopFlamethrower");
            return;
        }

        var mainModule = flamethrower.main;
        var extraModule = flamethrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;

        mainModule.duration = flamethrowDuration;
        extraModule.duration = flamethrowDuration;

        flamethrower.Clear();
        flamethrower.Play();
        bossSFX.flameSFX.Play();
    }

    // Activates the hammer attack
    public void ActivateHammer()
    {
        GameObject newActivation = ObjectPool.instance.GetObject(activationPrefab, impactPoint);
        ObjectPool.instance.ReturnObject(newActivation, 1);

        MassDamage(damagePoints[0].position, hammerCheckRadius, hammerActiveDamage);
    }

    // Checks if the boss can perform a jump attack
    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < minJumpDistanceRequired)
            return false;

        if (Time.time > lastTimeJump + jumpAttackCooldown && IsPlayerInClearSight())
            return true;

        return false;
    }

    // Executes the jump attack impact
    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint;

        if (impactPoint == null)
            impactPoint = transform;

        MassDamage(impactPoint.position, impactRadius, jumpAttackDamage);
        bossSFX.impactSFX.Play();
        jumpAttackVFX.Play();
    }

    // Deals damage to all entities within the impact radius
    private void MassDamage(Vector3 impactPoint, float impactRadius, int damage)
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius, ~whatIsAlly);

        foreach (Collider hit in colliders)
        {
            I_Damagable damagable = hit.GetComponent<I_Damagable>();
            if (damagable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;
                if (uniqueEntities.Add(rootEntity) == false)
                    continue;

                damagable.TakeDamage(damage);
            }
            ApplyPhysicalForce(impactPoint, impactRadius, hit);
        }
    }

    // Applies physical force to entities within the impact radius
    private void ApplyPhysicalForce(Vector3 impactPoint, float impactRadius, Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();

        if (rb != null)
            rb.AddExplosionForce(impactPower, impactPoint, impactRadius, upwardsMulti, ForceMode.Impulse);
    }

    // Checks if the boss can perform an ability
    public bool CanDoAbility()
    {
        bool playerWithinDistance = Vector3.Distance(transform.position, player.position) < minAbilityDistance;

        if (playerWithinDistance == false)
            return false;

        if (Time.time > lastTimeAbility + abilityCooldown && playerWithinDistance)
            return true;

        return false;
    }

    public void SetAbilityOnCooldown() => lastTimeAbility = Time.time;

    public void SetJumpAttackOnCooldown() => lastTimeJump = Time.time;
    #endregion

    #region Damage Methods
    public override void Die()
    {
        base.Die();

        if (!HealthController.muteDeathSound)
            bossSFX.deadSFX.Play();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);

        if (minimapIcon != null)
            minimapIcon.SetActive(false);

        // Change layer for enemy's lock-on part
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Enemy"));
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }
    #endregion
}