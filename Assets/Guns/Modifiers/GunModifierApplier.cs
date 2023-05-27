using Guns.Modifiers;
using System.Collections.Generic;
using UnityEngine;

public class GunModifierApplier : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;
    public List<object> upgradeList = new List<object>();

    private void Start()
    {
        DamageModifier damageModifier = new()
        {
            amount = 10f,
            attributeName = "damageConfig/DamageCurve",
            description = "Damage"
        };
        //upgradeList.Add(damageModifier);
        damageModifier.Apply(gunSelector.activeGun);
        //Debug.Log(gunSelector.activeGun.damageConfig.DamageCurve.constant);

        Vector3Modifier spreadModifier = new()
        {
            amount = Vector3.zero,
            attributeName = "shootConfig/spread",
            description = "Spread"
        };
        //upgradeList.Add(spreadModifier);
        spreadModifier.Apply(gunSelector.activeGun);

        FloatModifier textureSpreadMultiplierModifier = new()
        {
            amount = 0f,
            attributeName = "shootConfig/spreadMultiplier",
            description = "Spread Multiplier"
        };
        //upgradeList.Add(textureSpreadMultiplierModifier);
        textureSpreadMultiplierModifier.Apply(gunSelector.activeGun);
    }
}
