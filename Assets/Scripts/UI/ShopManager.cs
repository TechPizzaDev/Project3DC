using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    //Just temporary number. Should obviously be replaced by the Player's actual dollar/health count.
    public int dollars = 100;
    public int health = 80;

    public TMP_Text dollarUI;
    public TMP_Text healthUI;

    public ShopItem[] shopItem;
    public ShopTemplate[] shopTemplate;
    public GameObject[] shopTemplateGO;

    public Button[] buyBtn;



    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < shopItem.Length; i++)
        {
            shopTemplateGO[i].SetActive(true);
        }

        UIDisplay();
        CheckBuyability();
        LoadTemplates();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UIDisplay()
    {
        dollarUI.text = "$ " + dollars;
        healthUI.text = "HP " + health;
    }

    public void LoadTemplates()
    {
        for(int i = 0; i < shopItem.Length; i++)
        {
            shopTemplate[i].titleTxt.text = shopItem[i].title;
            shopTemplate[i].descriptionTxt.text = shopItem[i].description;
            shopTemplate[i].priceTxt.text = "$ " + shopItem[i].baseCost.ToString();

        }
    }

    public void CheckBuyability()
    {
        for(int i = 0; i < shopItem.Length; i++)
        {
            if(dollars >= shopItem[i].baseCost) //If you can afford it
            {
                buyBtn[i].interactable = true;
            }
            else
            {
                buyBtn[i].interactable = false;
            }
        }
    }


}
