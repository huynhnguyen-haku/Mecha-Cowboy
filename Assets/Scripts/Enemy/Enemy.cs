using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float turnSpeed;

    [Header("Idle Info")]
    public float idleTime;

    [Header("Movement Info")]
    public float moveSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Animator anim { get; private set; }

    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }


    protected virtual void Update()
    {
        
    }
    
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform point in patrolPoints)
        {
            point.parent = null;
        }
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime); 

        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

}
