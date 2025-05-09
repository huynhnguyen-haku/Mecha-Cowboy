using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;
    private Animator animator;

    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        RagdollActive(false);
    }

    public void RagdollActive(bool active)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = !active;
        
        if (animator != null)
            animator.enabled = !active;
    }

    public void ColliderActive(bool active)
    {
        foreach (Collider col in ragdollColliders)
            col.enabled = active;
    }
}