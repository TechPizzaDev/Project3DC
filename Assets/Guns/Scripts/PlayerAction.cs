using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;

    //public void OnShoot()
    //{
    //    gunSelector.activeGun.Tick();
    //}

    private void Update()
    {
        if (gunSelector.activeGun != null)
        {
            gunSelector.activeGun.Tick(Mouse.current.leftButton.isPressed);
        }
    }
}
