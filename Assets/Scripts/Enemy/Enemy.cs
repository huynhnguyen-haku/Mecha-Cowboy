using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int healthPoint = 15;

    [Header("Idle Info")]
    public float idleTime;
    public float arrgresssionRange;

    [Header("Movement Info")]
    public float moveSpeed;
    public float turnSpeed;
    public float chaseSpeed;


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

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
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

    }

    protected bool ShouldEnterBattleMode()
    {
        bool inAggresionRange = Vector3.Distance(transform.position, player.position) < arrgresssionRange;
        if (inAggresionRange && !inBattleMode)
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
        healthPoint--;
        EnterBattleMode();
    }
    public virtual void DeathImpact(Vector3 force, Vector3 hitpoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCourotine(force, hitpoint, rb));
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
    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

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
    #endregion

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arrgresssionRange);
    }
}

