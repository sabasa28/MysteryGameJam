using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneData : MonoBehaviour
{
    [SerializeField] List<GameObject> necessaryInteractions = new List<GameObject>();
    public Transform entrance;
    public Transform exit;
    public Transform exitDoor;
    public TextEntry zoneDoneChat;
    public bool allowHook;
    public bool allowBeacons;
    public bool allowFlashlight;

    public void CheckNecessaryInteractions()
    {
        for (int i = necessaryInteractions.Count - 1; i >= 0; i--)
        {
            if (!necessaryInteractions[i].GetComponent<IInteractable>().IsInteractable())
            {
                necessaryInteractions.RemoveAt(i);
                continue;
            }
        }
    }

    public bool GetClosestInteractable(Vector3 pos, out GameObject closestInteractable)
    {
        if (necessaryInteractions.Count == 0)
        {
            closestInteractable = null;
            return false;
        }
        float closestDist = 9999.9f; //very far!
        closestInteractable = null;
        foreach (GameObject interactable in necessaryInteractions)
        {
            float distToInteractable = Vector3.Distance(interactable.transform.position, pos);
            if (distToInteractable < closestDist)
            {
                closestDist = distToInteractable;
                closestInteractable = interactable;
            }
        }
        return closestInteractable != null;
    }

    public void RemoveFromNecessaryInteractions(GameObject gameObjectToRemove)
    {
        GameplayController.Get().OnCurrentZoneNecessaryInteractableFound(gameObjectToRemove);
        necessaryInteractions.Remove(gameObjectToRemove);
    }

    public bool HasNecessaryInteractionLeft()
    {
        return necessaryInteractions.Count > 0;
    }

    public void AddNecessaryInteraction(GameObject interactable)
    {
        necessaryInteractions.Add(interactable);
    }
}
