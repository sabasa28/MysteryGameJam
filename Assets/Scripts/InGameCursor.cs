using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameCursor : MonoBehaviour
{
    Vector2 minConstrain;
    Vector2 maxConstrain;
    Camera mainCamera;
    [SerializeField] float zOffset;
    Vector3 screenMousePos;
    bool isEnabled = false;
    [SerializeField] Image image;
    private void Start()
    {
        image.enabled = false;
    }
    public void EnableCursor(Vector2 newMinConstrain, Vector2 newMaxConstrain, float newZOffset)
    {
        minConstrain = newMinConstrain;
        maxConstrain = newMaxConstrain;
        mainCamera = Camera.main;
        zOffset = (int)(newZOffset * 1000.0f) / 1000.0f;
        isEnabled = true;
        image.enabled = true;
    }

    public void DisableCursor()
    {
        isEnabled = false;  
        image.enabled = false;
    }

    void Update()
    {
        if (!isEnabled)
        {
            return;
        }
        screenMousePos = Input.mousePosition;
        screenMousePos.z = zOffset;
        transform.position = mainCamera.ScreenToWorldPoint(screenMousePos);
        if (transform.localPosition.x < minConstrain.x)
        {
            transform.localPosition = new Vector3(minConstrain.x, transform.localPosition.y, transform.localPosition.z);
        }
        else if (transform.localPosition.x > maxConstrain.x)
        {
            transform.localPosition = new Vector3(maxConstrain.x, transform.localPosition.y, transform.localPosition.z);
        }
        if (transform.localPosition.y < minConstrain.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, minConstrain.y, transform.localPosition.z);
        }
        else if (transform.localPosition.y > maxConstrain.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, maxConstrain.y, transform.localPosition.z);
        }
    }
}
