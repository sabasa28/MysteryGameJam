using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDoc : MonoBehaviour, IInteractable
{
    enum ShipDocNum
    {
        Doc1,
        Doc2,
        Doc3
    }
    [SerializeField] ShipDocNum shipDocNum;
    public void Interact()
    {
        switch (shipDocNum)
        {
            case ShipDocNum.Doc1:
                UIGameplay.Get().DisplayShipDoc1();
                break;
            case ShipDocNum.Doc2:
                UIGameplay.Get().DisplayShipDoc2();
                break;
            case ShipDocNum.Doc3:
                UIGameplay.Get().DisplayShipDoc3();
                break;
        }
    }

    public bool IsInteractable()
    {
        return true;
    }

    public void RemoveFromNecessaryInteractables()
    {
    }
}
