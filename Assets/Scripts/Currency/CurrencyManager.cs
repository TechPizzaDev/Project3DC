using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public int dollars;

    public void AquireDollars(int amount) //MakeItRain()
    {
        dollars += amount;
    }

}
