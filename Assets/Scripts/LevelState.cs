using System.Collections;
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

    public float PlayerHealth;
    public float PlayerMaxHealth;
    public int PlayerCurrency;
}
