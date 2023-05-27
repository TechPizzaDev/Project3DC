using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private float bulletRange;
    [SerializeField] private float bulletSpread;
    [SerializeField] private float shootingCooldownTime = 3.0f;
    [SerializeField] private float shootingCooldown;
    [SerializeField]
    PlayerGunSelector gunSelector;

    public bool shootingAtPlayer;

    int bulletDamage = 20;

    //Player layer
    public LayerMask targetPlayer;

    public Transform targetTransform;

    void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        shootingCooldown = shootingCooldownTime;
    }

    public void CharacterShooting()
    {
        Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

        Debug.Log("Shot");
        gunSelector.activeGun.TryToShoot();

    }
}
