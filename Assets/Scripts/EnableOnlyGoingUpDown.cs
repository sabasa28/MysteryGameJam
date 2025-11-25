using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnlyGoingUpDown : MonoBehaviour
{
    [SerializeField] bool OnlyEnableGoingUp;
    void Start()
    {
        gameObject.SetActive(OnlyEnableGoingUp == LevelsManager.Get().GoingUp);
    }

}
