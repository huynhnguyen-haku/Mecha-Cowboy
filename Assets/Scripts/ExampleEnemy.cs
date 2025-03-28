using UnityEngine;
using UnityEngine.AI;

public class ExampleEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;

    public void Update()
    {
        agent.SetDestination(target.position);
    }
}
