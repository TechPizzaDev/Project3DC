using Guns.Modifiers;
using UnityEngine;

public class GunModifierApplier : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;

    void Start()
    {
        DamageModifier damageModifier = new()
        {
            amount = 1.5f,
            attributeName = "DamageConfig/damageCurve"
        };
        damageModifier.Apply(gunSelector.activeGun);

        Vector3Modifier spreadModifier = new()
        {
            amount = Vector3.zero,
            attributeName = "ShootConfig/spread"
        };
        spreadModifier.Apply(gunSelector.activeGun);
    }
}
