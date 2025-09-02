using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstuckCheckpoint : MonoBehaviour
{
    [SerializeField] Transform posForTP;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameplayController.Get().SetUnstuckCheckpoint(posForTP);
        }
    }
}
