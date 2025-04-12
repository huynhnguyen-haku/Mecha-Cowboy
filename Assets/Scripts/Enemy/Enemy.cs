using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public LayerMask whatIsAlly;
    public int healthPoint = 15;

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

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }
    public Enemy_Visual visual { get; private set; }
    public Enemy_Ragdoll ragdoll { get; private set; }
    public Enemy_Health health { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        health = GetComponent<Enemy_Health>();
        ragdoll = GetComponent<Enemy_Ragdoll>();
        visual = GetComponent<Enemy_Visual>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
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


    public virtual void GetHit()
    {
        EnterBattleMode();
        if (health.ShouldDie())
        {
            Die();
        }
        health.ReduceHealth();
    }

    public virtual void Die()
    {

    }

    public virtual void BulletImpact(Vector3 force, Vector3 hitpoint, Rigidbody rb)
    {
        if(health.ShouldDie())
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

