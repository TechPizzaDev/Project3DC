using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TextMeshProUGUI))]
public class AmmoDisplayer : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;
    private TextMeshProUGUI ammoText;

    private void Awake()
    {
        ammoText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        switch (gunSelector.activeGun.ammoConfig.ammoType)
        {
            case AmmoType.infinite:
                ammoText.SetText(
                    $"Åá / Åá"
                );
                break;
            case AmmoType.infiniteReserve:
                ammoText.SetText(
                    $"{gunSelector.activeGun.ammoConfig.currentClipAmmo} / " + $"Åá"
                );
                break;
            case AmmoType.normal:
                ammoText.SetText(
                    $"{gunSelector.activeGun.ammoConfig.currentClipAmmo} / " +
                    $"{gunSelector.activeGun.ammoConfig.currentAmmo}"
                );
                break;
            default:
                break;
        }


        
    }
}
