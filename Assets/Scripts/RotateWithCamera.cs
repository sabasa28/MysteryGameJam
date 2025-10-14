using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{
    void Update()
    {
        transform.up = -Camera.main.transform.forward;
    }
}
