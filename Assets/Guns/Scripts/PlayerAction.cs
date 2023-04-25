using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed && gunSelector.activeGun != null)
        {
            gunSelector.activeGun.Shoot();
        }
    }
}
