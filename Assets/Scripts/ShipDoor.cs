using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDoor : MonoBehaviour, IInteractable
{
    [SerializeField] float timeBetweenInteractions;
    bool interactable = true;
    [SerializeField] TextEntry cantExitLog;
    [SerializeField] TextEntry cantEnterLog;
    public void Interact()
    {
        GameplayController gc = GameplayController.Get();
        LevelsManager lvlmg = LevelsManager.Get();
        if (gc.IsInShip() && !lvlmg.persistentData.PlayerReadAnyLog())
        {
            ChatManager.Get().StartDisplayingTextEntry(cantExitLog);
        }
        else if (!gc.IsInShip() && (lvlmg.persistentData.isReturning && !gc.IsZoneDone()))
        {
            ChatManager.Get().StartDisplayingTextEntry(cantEnterLog);
        }
        else
        {
            gc.MovePlayerInOutOfShip();
        }
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
