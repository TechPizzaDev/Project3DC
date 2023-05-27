using UnityEngine;

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
