using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{
    bool found = false;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip foundSound;
    public void Interact()
    {
        if (!found)
        {
            PlayFoundAnim();
        };
    }

    public bool IsInteractable()
    {
        return !found;
    }

    void PlayFoundAnim()
    {
        animator.SetTrigger("Found");
    }

    public void PlayFoundSound()
    {
        AudioManager.Get().PlaySFX(foundSound, 2);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void RemoveFromNecessaryInteractables()
    {
    }
}
