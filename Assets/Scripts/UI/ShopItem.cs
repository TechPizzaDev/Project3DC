using Guns.Modifiers;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Shop/Shop Item", order = 1)]
public class ShopItem : ScriptableObject
{
    public UpgradeType upgradeType = UpgradeType.damage;
    public float floatAmount = 0f;
    public Vector3 vector3Amount = Vector3.zero;

    public string title, description;
    public int baseCost;
}
