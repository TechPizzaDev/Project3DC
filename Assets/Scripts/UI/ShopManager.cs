using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Guns.Modifiers;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class manages the functionality of the shop in the game.
/// It handles UI display, loading items and templates, checking buyability,
/// purchasing items, and applying upgrades to the player's gun based on the
/// selected shop item.
/// </summary>

public class ShopManager : MonoBehaviour
{

    public TMP_Text dollarUI;
    public TMP_Text healthUI;

    public ShopItem[] allItems; 
    public ShopItem[] shopItems;
    public ShopTemplate[] shopTemplate;
    public GameObject[] shopTemplateGO;

    public Button[] buyBtn;

    public UnitHealth unitHealth;
    public PlayerGunSelector gunSelector;

    void Start()
    {
        // Activates shop templates for selected shop items
        // Loads items, loads templates, and checks buyability

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopTemplateGO[i].SetActive(true);
        }

        LoadItems();
        LoadTemplates();
        //CheckBuyability();
        EnableItems();
        DisableItems();
    }

    private void LoadItems()
    {
        allItems = Resources.LoadAll<ShopItem>("ShopItems/");

        //assign shop items to random items from all items
        for (int i = 0; i < shopItems.Length;)
        {
            int rnd = Random.Range(0, allItems.Length);

            if (!shopItems.Contains(allItems[rnd]))
            {
                shopItems[i] = allItems[rnd];
                i++;
            }
        }
    }

    void Update()
    {
        // Updates the UI display
        UIDisplay();
    }

    /// <summary>
    /// Updates the dollar and health UI texts with current values. 
    /// </summary>
    public void UIDisplay()
    {
        dollarUI.text = "$ " + unitHealth.currency;
        healthUI.text = "HP " + unitHealth.health;
    }

    /// <summary>
    ///  Loads shop item data into the shop templates.
    /// </summary>

    public void LoadTemplates()
    {
        // Sets the title, description, and price texts of shop templates based on shop items
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopTemplate[i].titleTxt.text = shopItems[i].title;
            shopTemplate[i].descriptionTxt.text = shopItems[i].description;
            shopTemplate[i].priceTxt.text = "$ " + shopItems[i].baseCost.ToString();

        }
    }

    /// <summary>
    ///  Checks if shop items can be bought and enables/disables buy buttons accordingly.
    /// </summary>

    public void EnableItems()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (unitHealth.currency >= shopItems[i].baseCost)
            {
                buyBtn[i].interactable = true;
            }
        }
    }

    public void DisableItems()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (unitHealth.currency < shopItems[i].baseCost)
            {
                buyBtn[i].interactable = false;
            }
        }
    }

    public void CheckBuyability()
    {
        // Checks if the player has enough currency to buy each shop item
        // Enables or disables buy buttons based on buyability

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (unitHealth.currency < shopItems[i].baseCost)
            {
                buyBtn[i].interactable = false;
            }
            else
            {
                buyBtn[i].interactable = true;
            }
        }
    }

    /// <summary>
    /// Handles the purchase of a shop item
    /// </summary>
    public void PurchaseItem(int btnNo)
    {
        // Reduces player's currency, updates buyability, and applies upgrade of the selected shop item
        unitHealth.currency = unitHealth.currency - shopItems[btnNo].baseCost;
        //CheckBuyability();
        DisableItems();
        ApplyUpgrade(shopItems[btnNo]);
        buyBtn[btnNo].interactable = false;
    }

    /// <summary>
    /// Restores the health of the player based on which button they press.
    /// </summary>
    public void RestoreHealth(int healthToRestore)
    {
        if (unitHealth.health + healthToRestore <= unitHealth.maxHealth)
        {
            unitHealth.health += healthToRestore;
        }
        else
        {
            unitHealth.health = unitHealth.maxHealth;
        }
    }

    /// <summary>
    /// Applies the upgrade of the selected shop item to the player's gun
    /// </summary>
    public void ApplyUpgrade(ShopItem item)
    {
        // Applies different modifiers to the player's gun based on the upgrade type of the shop item
        switch (item.upgradeType)
        {
            case
            UpgradeType.damage:
                DamageModifier damageModifier = new()
                {
                    amount = item.floatAmount,
                    attributeName = "damageConfig/DamageCurve",
                    description = "Increases damage by " + item.floatAmount * 100 + "%"
                };
                damageModifier.Apply(gunSelector.activeGun);
                break;
            case
            UpgradeType.spread:
                Vector3Modifier spreadModifier = new()
                {
                    amount = item.vector3Amount,
                    attributeName = "shootConfig/Spread",
                    description = "Spread"
                };
                spreadModifier.Apply(gunSelector.activeGun);
                break;
            case
            UpgradeType.fireRate:
                FloatModifier fireRateModifier = new()
                {
                    amount = item.floatAmount,
                    attributeName = "shootConfig/fireRate",
                    description = "Fire Rate"
                };
                fireRateModifier.Apply(gunSelector.activeGun);
                break;
            case
            UpgradeType.magSize:
                FloatModifier magSizeModifier = new()
                {
                    amount = item.floatAmount,
                    attributeName = "ammoConfig/clipSize",
                    description = "Mag Size"
                };
                magSizeModifier.Apply(gunSelector.activeGun);
                break;
            //case
            //UpgradeType.reloadTime:
            //    FloatModifier reloadTimeModifier = new()
            //    {
            //        amount = item.floatAmount,
            //        attributeName = "shootConfig/reloadTime",
            //        description = "Reload Time"
            //    };
            //    reloadTimeModifier.Apply(gunSelector.activeGun);
            //    break;
            case
            UpgradeType.bulletSpeed:
                FloatModifier bulletSpeedModifier = new()
                {
                    amount = item.floatAmount,
                    attributeName = "shootConfig/bulletSpawnForce",
                    description = "Bullet Speed"
                };
                bulletSpeedModifier.Apply(gunSelector.activeGun);
                break;
            default:
                break;
        }
    }


}
