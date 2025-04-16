using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();
    private Interactable closestInteracble;


    private void Start()
    {
        Player player = GetComponent<Player>();
        player.controls.Character.Interact.performed += ctx => InteractWithClosest();   
    }

    public void UpdateClosestInteracble()
    {
        closestInteracble?.Highlight(false);

        closestInteracble = null;
        float closestDistance = float.MaxValue;

        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteracble = interactable;
            }
        }

        closestInteracble?.Highlight(true);
    }

    private void InteractWithClosest()
    {
        closestInteracble?.Interact();
        interactables.Remove(closestInteracble);

        UpdateClosestInteracble();
    }

    public List<Interactable> GetInteractables()
    {
        return interactables;
    }
}

