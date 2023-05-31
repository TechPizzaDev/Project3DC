using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    UnitHealth unitHealth;

    void Awake()
    {
        unitHealth = GetComponent<UnitHealth>();
    }

    public void AquireDollars(int amount)
    {
        unitHealth.currency += amount;
    }
}
