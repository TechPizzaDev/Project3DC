using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private float bulletRange;
    [SerializeField] private float bulletSpread;
    [SerializeField] private float shootingCooldownTime = 3.0f;
    [SerializeField] private float shootingCooldown;

    public bool shootingAtPlayer;

    int bulletDamage = 20;

    //Player layer
    public LayerMask targetPlayer;

    public GameObject player;
    PlayerHealth playerHealth;
    void Start()
    {
        shootingCooldown = shootingCooldownTime;
        playerHealth = player.GetComponent<PlayerHealth>();

    }

    void Update()
    {
        //Vector3 playerTarget = (player.transform.position - transform.position).normalized;

        //if(shootingCooldown <= 0 && shootingAtPlayer)
        //{
        //    if (Vector3.Angle(transform.forward, playerTarget) < bulletSpread / 2)
        //    {
        //        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
        //        if (distanceToTarget <= bulletRange)
        //        {
        //            //If there is no obstacle in the way = enemy has detected the player. 
        //            if (Physics.Raycast(transform.position, playerTarget, distanceToTarget, targetPlayer))
        //            {
        //                Debug.Log("Shot");
        //                playerHealth.TakeDamage(bulletDamage);
        //            }
        //        } 
        //    }

        //    shootingCooldown = shootingCooldownTime;

        //}
        //else if(shootingCooldown > 0 && shootingAtPlayer)
        //{
        //    shootingCooldown -= Time.deltaTime; 
        //}
        //else
        //{
        //    shootingCooldown = shootingCooldownTime;
        //}
    }
    public void CharacterShooting()
    {
        Vector3 playerTarget = (player.transform.position - transform.position).normalized;

        Debug.Log("BOOM");
        if (Vector3.Angle(transform.forward, playerTarget) < bulletSpread / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToTarget <= bulletRange)
            {
                //If there is no obstacle in the way = enemy has detected the player. 
                if (Physics.Raycast(transform.position, playerTarget, distanceToTarget, targetPlayer))
                {
                    Debug.Log("Shot");
                    playerHealth.TakeDamage(bulletDamage);
                }
            }
        }
    }
}
