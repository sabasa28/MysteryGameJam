using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDoc : MonoBehaviour, IInteractable
{
    enum ShipDocNum
    {
        Doc1,
        Doc2,
        Doc3,
        Doc4
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
            case ShipDocNum.Doc4:
                UIGameplay.Get().DisplayShipDoc4();
                break;
        }
    }

    public bool IsInteractable()
    {
        if (shipDocNum != ShipDocNum.Doc4)
        {
            return true;
        }
        else
        {
            return LevelsManager.Get().persistentData.canEndGame;
        }
    }

    public void RemoveFromNecessaryInteractables()
    {
    }
}
