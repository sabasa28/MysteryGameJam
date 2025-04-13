using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    Vector3 targetPos;
    public bool hookReachedTarget;
    bool hooking;
    [SerializeField] float hookThrowSpeed;
    float initialZScale;
    float targetZScale;
    bool successfulHit;

    private void Awake()
    {
        initialZScale = transform.localScale.z;
    }

    public void SetTargetPos(Vector3 inTargetPos, bool inSuccessfulHit = true)
    {
        successfulHit = inSuccessfulHit;
        targetPos = inTargetPos;
        hooking = true;
        StopCoroutine(ScaleHook());
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, initialZScale);
        hookReachedTarget = false;
        targetZScale = Vector3.Distance(targetPos, transform.position) * 10;
        StartCoroutine(ScaleHook());
    }

    IEnumerator ScaleHook()
    {
        while (hooking)
        {
            //agregar algo para que haya una minima distancia entre escalas en la que la escala no se modifica
            targetZScale = Vector3.Distance(targetPos, transform.position) * 10;
            if (!hookReachedTarget)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z * (1 + hookThrowSpeed));
            }
            if (hookReachedTarget || transform.localScale.z >= targetZScale)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, targetZScale);
                hookReachedTarget = true;
                if (!successfulHit)
                {
                    StartCoroutine(UnscaleHook());
                    break;
                }
            }

            transform.LookAt(targetPos, Vector3.up);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator UnscaleHook()
    {
        targetZScale = initialZScale;
        while (transform.localScale.z > targetZScale)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z * (1 - hookThrowSpeed));
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, targetZScale);
        gameObject.SetActive(false);
    }

}
