using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;
    [SerializeField]
    private bool autoReload = true;
    [SerializeField]
    private Animator playerAnimator;

    private bool isReloading;

    //public void OnShoot()
    //{
    //    gunSelector.activeGun.Tick();
    //}

    private void Update()
    {
        if (gunSelector.activeGun != null
            && !isReloading)
        {
            gunSelector.activeGun.Tick(Mouse.current.leftButton.isPressed);
        }

        if (ShouldManualReload() || ShouldAutoReload())
        {
            gunSelector.activeGun.StartReloading();
            isReloading = true;
            playerAnimator.SetTrigger("Reload");
        }
    }

    private void EndReload()
    {
        gunSelector.activeGun.EndReload();
        isReloading = false;
    }

    private bool ShouldManualReload()
    {
        return !isReloading 
            && Keyboard.current.rKey.wasPressedThisFrame
            && gunSelector.activeGun.CanReload();
    }

    private bool ShouldAutoReload()
    {
        return !isReloading 
            && autoReload
            && gunSelector.activeGun.ammoConfig.currentClipAmmo == 0
            && gunSelector.activeGun.CanReload();
    }
}
