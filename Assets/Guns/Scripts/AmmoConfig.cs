using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Configuration", order = 3)]
public class AmmoConfig : ScriptableObject, System.ICloneable
{
    public AmmoType ammoType = AmmoType.normal;

    public int maxAmmo = 120;
    public int clipSize = 30;

    public int currentAmmo = 120;
    public int currentClipAmmo = 30;

    /// <summary>
    /// Reloads weapon conserving ammo left in the magazine.
    /// Meaning it will only subtract the difference between clipSize and currentClipAmmo from currentAmmo
    /// If ammoType is AmmoType.normal, other ammo types do not subtract ammo from currentAmmo
    /// </summary>
    public void Reload()
    {
        int maxReloadAmount = Mathf.Min(clipSize, currentAmmo);
        int availableBulletsInCurrentClip = clipSize - currentClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletsInCurrentClip);

        currentClipAmmo = currentClipAmmo + reloadAmount;
        if (ammoType == AmmoType.normal)
        {
            currentAmmo -= reloadAmount;
        }
    }

    /// <summary>
    /// Checks if the weapon can be reloaded
    /// </summary>
    /// <returns></returns>
    public bool CanReload()
    {
        return currentClipAmmo < clipSize && currentAmmo > 0;
    }

    /// <summary>
    /// Creates a new instance of AmmoConfig and copies the values from this instance to the new one
    /// </summary>
    public object Clone()
    {
        AmmoConfig config = CreateInstance<AmmoConfig>();

        Utilities.CopyValues(this, config);

        return config;
    }
    
}
