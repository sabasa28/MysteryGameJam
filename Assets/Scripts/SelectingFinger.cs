using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingFinger : MonoBehaviour
{
    [SerializeField] float distToCamera;
    bool animating = false;
    public float animationTime = 0.25f;
    float animationSpeed;
    private void OnEnable()
    {
        transform.forward = Camera.main.transform.forward;
        StartCoroutine(Animate(true));
        animationSpeed = 1 / animationTime;
    }

    public void AnimateAndDisable()
    {
        StartCoroutine(Animate(false));
    }

    void Update()
    {
        if (!animating)
        {
            Vector3 mouseVec = Input.mousePosition;
            mouseVec.z = distToCamera;
            mouseVec = Camera.main.ScreenToWorldPoint(mouseVec);
            transform.position = mouseVec;
        }
    }
    IEnumerator Animate(bool enterScreen)
    { 
        animating = true;

        Vector3 initialMousePos = Input.mousePosition;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * animationSpeed;
            Vector3 mouseVec;
            if (enterScreen)
            {
                mouseVec = Vector3.Lerp(Vector3.zero, Input.mousePosition, t);
            }
            else
            {
                mouseVec = Vector3.Lerp(initialMousePos, Vector3.zero, t);
            }
            mouseVec.z = distToCamera;
            mouseVec = Camera.main.ScreenToWorldPoint(mouseVec);
            transform.position = mouseVec;
            yield return null;
        }
        animating = false;
        
        if (!enterScreen)
        {
            gameObject.SetActive(false);
        }
    }
}
