using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField]
    private GunType Gun;
    [SerializeField]
    private Transform gunParent;
    [SerializeField]
    private List<Gun> guns;

    [Space]
    [Header("Runtime Filled")]
    public Gun activeGun;

    private void Start()
    {
        Gun gun = guns.Find(gun => gun.type == Gun);

        if (gun == null)
        {
            Debug.LogError($"No Gun found for GunType: {gun}");
            return;
        }

        activeGun = gun.Clone() as Gun;
        activeGun.Spawn(gunParent, this);
    }
}
