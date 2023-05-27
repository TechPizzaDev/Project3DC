using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Guns.Modifiers;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    //Just temporary number. Should obviously be replaced by the Player's actual dollar/health count.
    public TMP_Text dollarUI;
    public TMP_Text healthUI;

    public ShopItem[] allItems; 
    public ShopItem[] shopItem;
    public ShopTemplate[] shopTemplate;
    public GameObject[] shopTemplateGO;

    public Button[] buyBtn;

    public UnitHealth unitHealth;
    public PlayerGunSelector gunSelector;

    

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < shopItem.Length; i++)
        {
            shopTemplateGO[i].SetActive(true);
        }

        LoadItems();
        LoadTemplates();
        CheckBuyability();
    }

    private void LoadItems()
    {
        allItems = Resources.LoadAll<ShopItem>("ShopItems/");

        //assign shop items to random items from all items
        for (int i = 0; i < shopItem.Length;)
        {
            int rnd = Random.Range(0, allItems.Length);

            if (!shopItem.Contains(allItems[rnd]))
            {
                shopItem[i] = allItems[rnd];
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UIDisplay();
    }

    public void UIDisplay()
    {
        dollarUI.text = "$ " + unitHealth.currency;
        healthUI.text = "HP " + unitHealth.health;
    }

    public void LoadTemplates()
    {
        for (int i = 0; i < shopItem.Length; i++)
        {
            shopTemplate[i].titleTxt.text = shopItem[i].title;
            shopTemplate[i].descriptionTxt.text = shopItem[i].description;
            shopTemplate[i].priceTxt.text = "$ " + shopItem[i].baseCost.ToString();

        }
    }

    public void CheckBuyability()
    {
        for (int i = 0; i < shopItem.Length; i++)
        {
            if (unitHealth.currency < shopItem[i].baseCost) //If you can't afford it
            {
                buyBtn[i].interactable = false;
            }
            else
            {
                buyBtn[i].interactable = true;
            }
        }
    }

    public void PurchaseItem(int btnNo)
    {
        unitHealth.currency = unitHealth.currency - shopItem[btnNo].baseCost;
        CheckBuyability();
        ApplyUpgrade(shopItem[btnNo]);
        buyBtn[btnNo].interactable = false;
    }

    public void ApplyUpgrade(ShopItem item)
    {
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
                Debug.Log(gunSelector.activeGun.damageConfig.DamageCurve.constant);
                break;
            case
            UpgradeType.spread:
                Vector3Modifier spreadModifier = new()
                {
                    amount = item.vector3Amount,
                    attributeName = "shootConfig/spread",
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
