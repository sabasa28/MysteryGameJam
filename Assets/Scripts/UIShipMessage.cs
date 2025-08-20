using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//esto no se usa, lo use para crear el prefab
public class UIShipMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TitleText;
    [SerializeField] TextMeshProUGUI DateText;
    [SerializeField] Button button;
    public void GenerateContent(uint messageNum, string daySent, uint monthSent, uint yearSent)
    {
        TitleText.text = "Earth update #" + messageNum;
        DateText.text = "Date: " + GetMonthFromNum(monthSent) + " " + daySent + ", " + yearSent;
        button.interactable = false;
    }


    string GetMonthFromNum(uint num)
    {
        switch (num)
        {
            case 0: //just go with it
                return "Dec";
            case 1:
                return "Jan";
            case 2:
                return "Feb";
            case 3:
                return "Mar";
            case 4:
                return "Apr";
            case 5:
                return "May";
            case 6:
                return "Jun";
            case 7:
                return "Jul";
            case 8:
                return "Aug";
            case 9:
                return "Sep";
            case 10:
                return "Oct";
            case 11:
                return "Nov";
            default:
                return "AAAAAAAAAAAAAAAAAAAAAAAAAA";
        }
    }
    /*
                 case 1:
                return "January";
            case 2:
                return "February";
            case 3:
                return "March";
            case 4:
                return "April";
            case 5:
                return "May";
            case 6:
                return "June";
            case 7:
                return "July";
            case 8:
                return "August";
            case 9:
                return "September";
            case 10:
                return "October";
            case 11:
                return "November";
            case 12:
                return "December";
            default:
                return "AAAAAAAAAAAAAAAAAAAAAAAAAA";
     */
}
