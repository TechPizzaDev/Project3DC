using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static LevelState Instance
    {
        get
        {
            return ScreenManager.LevelStateInstance.GetComponent<LevelState>();
        }
    }

    public List<ShopItem> AcquiredShopItems { get; } = new();

    public float PlayerHealth = 100;
    public float PlayerMaxHealth = 100;
    public int PlayerCurrency = 0;
}

