using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDoor : MonoBehaviour, IInteractable
{
    [SerializeField] float timeBetweenInteractions;
    bool interactable = true;
    public void Interact()
    {
        GameplayController.Get().MovePlayerInOutOfShip();
        StartCoroutine(InteractCooldown());
    }

    public bool IsInteractable()
    {
        return interactable;
    }

    IEnumerator InteractCooldown()
    {
        interactable = false;
        yield return new WaitForSeconds(timeBetweenInteractions);
        interactable = true;
    }

    public void RemoveFromNecessaryInteractables()
    {
        return;
    }

}
