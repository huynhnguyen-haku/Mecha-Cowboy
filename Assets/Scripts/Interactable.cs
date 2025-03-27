using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    protected Material defaultMaterial;
    protected MeshRenderer meshRenderer;



    private void Start()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
        defaultMaterial = meshRenderer.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        meshRenderer = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting with " + name);
    }


    public void Highlight(bool active)
    {
        if (active)
        {
            meshRenderer.material = highlightMaterial;
        }
        else
        {
            meshRenderer.material = defaultMaterial;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
        {
            return;
        }
        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteracble();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction == null)
        {
            return;
        }
        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteracble();
    }
}
