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

    public void CheckNecessaryInteractions()
    {
        for (int i = necessaryInteractions.Count - 1; i >= 0; i--)
        {
            if (!necessaryInteractions[i].activeInHierarchy)
            {
                necessaryInteractions.RemoveAt(i);
            }
        }
    }

    public bool GetClosestInteractable(Vector3 pos, out Vector3 interactablePos)
    {
        if (necessaryInteractions.Count == 0)
        {
            interactablePos = Vector3.zero;
            return false;
        }
        float closestDist = 9999.9f; //very far!
        interactablePos = Vector3.zero;
        foreach (GameObject interactable in necessaryInteractions)
        {
            float distToInteractable = Vector3.Distance(interactable.transform.position, pos);
            if (distToInteractable < closestDist)
            {
                closestDist = distToInteractable;
                interactablePos = interactable.transform.position;
            }
        }
        return closestDist != 9999.9f;
    }

    public void RemoveFromNecessaryInteractions(GameObject gameObjectToRemove)
    {
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
