using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;
    [SerializeField]
    private bool autoReload = true;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Image crosshair;
    [SerializeField]
    private GameObject gunHolder;

    private bool isReloading;

    private void Start()
    {
        playerAnimator = gunHolder.GetComponentInChildren<Animator>();
        Debug.Log(playerAnimator);
        //playerAnimator = gunSelector.activeGun.GetComponentInChildren<Animator>();
    }
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

        UpdateCrosshair();
    }

    private void UpdateCrosshair()
    {
        if (gunSelector.activeGun.shootConfig.shootType == ShootType.fromGun)
        {
            Vector3 gunTipPoint = gunSelector.activeGun.GetRaycastOrigin();
            Vector3 gunForward = gunSelector.activeGun.GetGunForward();
            Vector3 hitPoint = gunTipPoint + gunForward * 10;

            if (Physics.Raycast(
                gunTipPoint,
                gunForward,
                out RaycastHit hit,
                float.MaxValue,
                gunSelector.activeGun.shootConfig.hitMask))
            {
                hitPoint = hit.point;
            }

            Vector3 screenSpaceLocation = gunSelector.camera.WorldToScreenPoint(hitPoint);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)crosshair.transform.parent,
                screenSpaceLocation,
                null,
                out Vector2 localPosition))
            {
                crosshair.rectTransform.anchoredPosition = localPosition;
            }
            else
            {
                crosshair.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void EndReload()
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
