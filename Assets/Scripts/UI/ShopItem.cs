using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable Objects/ New Shop Item", order = 1)]
public class ShopItem : ScriptableObject
{
    public string title, description;
    public int baseCost;
}
