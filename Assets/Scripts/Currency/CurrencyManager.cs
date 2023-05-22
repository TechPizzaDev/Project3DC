using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public int dollars;
    public GameObject player;

    public void AquireDollars(int amount) //MakeItRain()
    {
        dollars += amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            AquireDollars(10);
        }
    }
}
