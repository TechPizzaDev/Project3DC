using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    //Just temporary number. Should obviously be replaced by the Player's actual dollar/health count.
    public TMP_Text dollarUI;
    public TMP_Text healthUI;

    public ShopItem[] shopItem;
    public ShopTemplate[] shopTemplate;
    public GameObject[] shopTemplateGO;

    public Button[] buyBtn;

    public UnitHealth unitHealth;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < shopItem.Length; i++)
        {
            shopTemplateGO[i].SetActive(true);
        }

        CheckBuyability();
        LoadTemplates();
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

    }


}
