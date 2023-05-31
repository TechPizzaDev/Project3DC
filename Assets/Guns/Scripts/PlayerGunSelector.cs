using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    public Camera camera;
    [SerializeField]
    private GunType Gun;
    [SerializeField]
    private Transform gunParent;
    [SerializeField]
    private List<Gun> guns;

    [Space]
    [Header("Runtime Filled")]
    public Gun activeGun;

    private void Awake()
    {
        Gun gun = guns.Find(gun => gun.type == Gun);

        if (gun == null)
        {
            Debug.LogError($"No Gun found for GunType: {gun}");
            return;
        }

        activeGun = gun.Clone() as Gun;
        activeGun.Spawn(gunParent, this, camera);
    }

    private void Start()
    {
        foreach (ShopItem item in LevelState.Instance.AcquiredShopItems)
        {
            ShopManager.ApplyUpgrade(this, item);
        }
    }
}
