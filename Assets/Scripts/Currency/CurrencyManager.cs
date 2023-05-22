using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CurrencyManager : MonoBehaviour
{
    UnitHealth unitHealth;

    public void Start()
    {
        unitHealth = GetComponent<UnitHealth>();
    }


    public void AquireDollars(int amount) //MakeItRain()
    {
        unitHealth.currency += amount;
    }

}
