using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glasses : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject glassesGlass;
    bool grabbed = false;
    private void Start()
    {
        if (LevelsManager.Get().GoingUp) gameObject.SetActive(false);
    }
    public void Interact()
    {
        DisableModel();
        grabbed = true;
        GameplayController.Get().SetNoGlassesVolumeState(false);
        StartCoroutine(PlayChat());
    }

    void DisableModel()
    {
        glassesGlass.SetActive(false);
        GetComponent<MeshRenderer>().enabled = false;
    }

    IEnumerator PlayChat()
    {
        yield return new WaitForSeconds(1.0f);
        ChatManager.Get().PlayGlassesFoundChat();
        GameplayController.Get().PutGlassesOnPlayer();
        gameObject.SetActive(false);
    }
    public bool IsInteractable()
    {
        return !grabbed;
    }

    public void RemoveFromNecessaryInteractables()
    {
    }

}
