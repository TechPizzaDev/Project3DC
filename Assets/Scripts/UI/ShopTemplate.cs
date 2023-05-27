using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class represents a template for displaying shop item information in the UI.
/// It contains references to the title, description, and price text components.
/// </summary>

public class ShopTemplate : MonoBehaviour
{
    public TMP_Text titleTxt; // Text component for displaying the title of the shop item
    public TMP_Text descriptionTxt; // Text component for displaying the description of the shop item
    public TMP_Text priceTxt; // Text component for displaying the price of the shop item
}
