using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICOCKpitMessages : MonoBehaviour
{
    [SerializeField] uint messagesToGenerate;
    [SerializeField] uint initialMonth;
    [SerializeField] string messagesDaySent;
    [SerializeField] uint initialYear;
    [SerializeField] UIShipMessage prefabToGenerate;
    [SerializeField] ScrollRect scroll;
    [SerializeField] bool generate;
    void GenerateMessages() //usar para crear prefab
    {
        for (uint i = 1; i <= messagesToGenerate; i++)
        {
            UIShipMessage GeneratedMessage = Instantiate(prefabToGenerate, transform);
            GeneratedMessage.GenerateContent(i, messagesDaySent, ((initialMonth + i - 1) % 12), initialYear + ((initialMonth + i - 2) / 12));
        }
    }
    private void Start()
    {
        if (generate)
        {
            GenerateMessages();
        }
        scroll.verticalNormalizedPosition = 0.0f;
    }

}
