using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    //Just temporary number. Should obviously be replaced by the Player's actual dollar/health count.
    public int dollars = 1000;
    public int health = 80;
    public TMP_Text dollarUI;
    public TMP_Text healthUI;

    // Start is called before the first frame update
    void Start()
    {
        UIDisplay();
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
}
