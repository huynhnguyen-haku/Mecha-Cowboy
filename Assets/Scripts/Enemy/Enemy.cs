using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Melee,
    Range,
    Boss
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public LayerMask whatIsAlly;
    public LayerMask whatIsPlayer;
    public EnemyType enemyType;

    [Space]

    [Header("Idle Info")]
    public float idleTime;
    public float arrgresssionRange;

    [Header("Movement Info")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;
    public float turnSpeed;


    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;


    private int currentPatrolIndex;
    public bool inBattleMode { get; private set; }
    protected bool isMeleeAttackReady;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }
    public Enemy_Visual visual { get; private set; }
    public Ragdoll ragdoll { get; private set; }
    public Enemy_Health health { get; private set; }
    public Enemy_DropController dropController { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        health = GetComponent<Enemy_Health>();
        ragdoll = GetComponent<Ragdoll>();
        visual = GetComponent<Enemy_Visual>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dropController = GetComponent<Enemy_DropController>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }


    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    protected virtual void InitializePerk()
    {

    }

    public virtual void MakeEnemyStronger()
    {
        int addtionalHealth = Mathf.RoundToInt(health.currentHealth * 1.5f);
        health.currentHealth += addtionalHealth;

        transform.localScale = transform.localScale * 1.5f;
    }

    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAgrressionRage() && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }
        return false;
    }
    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }


    public virtual void GetHit(int damage)
    {
        health.ReduceHealth(damage);

        if (health.EnemyShouldDie())
        {
            Die();
        }

        EnterBattleMode();
    }


    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject FX, int damage)
    {
        if (isMeleeAttackReady == false)
            return;

        foreach (Transform damagePoint in damagePoints)
        {
            Collider[] detectedHits =
                Physics.OverlapSphere(damagePoint.position, attackCheckRadius, whatIsPlayer);


                for (int i = 0; i < detectedHits.Length; i++)
            {
                I_Damagable damagable = detectedHits[i].GetComponent<I_Damagable>();

                if (damagable != null)
                {
                    damagable.TakeDamage(damage);
                    isMeleeAttackReady = false;
                    GameObject newAttackFX = ObjectPool.instance.GetObject(FX, damagePoint);

                    ObjectPool.instance.ReturnObject(newAttackFX, 1);
                    return;
                }
            }
        }
    }

    public void EnableMeleeAttack(bool enable)
    {
        isMeleeAttackReady = enable;
    }

    public virtual void Die()
    {
        dropController.DropItems();
        MissionObject_Target huntTarget = GetComponent<MissionObject_Target>();
        huntTarget?.InvokeOnTargetKilled();
    }

    public virtual void BulletImpact(Vector3 force, Vector3 hitpoint, Rigidbody rb)
    {
        if(health.EnemyShouldDie())
        {
            StartCoroutine(DeathImpactCourotine(force, hitpoint, rb));

        }
    }

    private IEnumerator DeathImpactCourotine(Vector3 force, Vector3 hitpoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForceAtPosition(force, hitpoint, ForceMode.Impulse);
    }

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    public void FaceTarget(Vector3 target, float turnSpeed = 0)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        if (turnSpeed == 0)
        {
            turnSpeed = this.turnSpeed;
        }

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }


    #region Animation Events
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;
    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    #endregion

    #region Patrol
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    public bool IsPlayerInAgrressionRage()
    {
        return Vector3.Distance(transform.position, player.position) < arrgresssionRange;
    }

    #endregion

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arrgresssionRange);
    }
}

