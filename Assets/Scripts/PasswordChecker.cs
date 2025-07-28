using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasswordChecker : MonoBehaviour
{
    Animator animator;
    [SerializeField] TextMeshProUGUI synchronizingText;
    public bool idle = true;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (GameplayController.Get().TabletEnabled())
        {
            animator.SetTrigger("Synchronized");
        }
        synchronizingText.text = "Synchronizing tablet";
    }

    public void SuccessfulPassword()
    {
        idle = false;
        animator.SetTrigger("Success");
    }

    public void FailedPassword()
    {
        idle = false;
        animator.SetTrigger("Fail");
    }

    public void AddDotToSynchronizingText()
    {
        synchronizingText.text += ".";
    }

    public void BackToIdle()
    {
        idle = true;
    }
}
