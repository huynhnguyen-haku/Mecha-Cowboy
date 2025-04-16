using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    protected Material defaultMaterial;
    protected MeshRenderer meshRenderer;

    protected Player_WeaponController weaponController;




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
        if (weaponController == null)
        {
            weaponController = other.GetComponent<Player_WeaponController>();
        }

        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();

        if (playerInteraction == null)
        {
            return;
        }
        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteracble();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();
        if (playerInteraction == null)
        {
            return;
        }
        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteracble();
    }
}
