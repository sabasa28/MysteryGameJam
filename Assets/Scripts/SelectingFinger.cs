using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingFinger : MonoBehaviour
{
    [SerializeField] float distToCamera;
    private void OnEnable()
    {
        transform.forward = Camera.main.transform.forward;
    }
    void Update()
    {
        Vector3 mouseVec = Input.mousePosition;
        mouseVec.z = distToCamera;
        mouseVec = Camera.main.ScreenToWorldPoint(mouseVec);
        transform.position = mouseVec; 
    }
}
