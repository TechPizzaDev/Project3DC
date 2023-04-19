using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoombugExplode : MonoBehaviour
{
    int explosionDamage = 45;
    float damageRange = 3;
    BoombugMovement enemyMovement;
    EnemyDetection enemyDetection;
    [SerializeField]
    GameObject player;
    PlayerHealth playerHealth;
    [SerializeField] float timer = 1f;
    public bool explosionMode;

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();    
        enemyMovement = GetComponent<BoombugMovement>();  
        playerHealth = player.GetComponent<PlayerHealth>();
        
    }

    void Update()
    {
        if(explosionMode)
        {
            timer -= Time.deltaTime;

        }
        if (timer <= 0)
        {
            float distanceToPlayer = Vector3.Distance(this.transform.position, enemyMovement.movePosTransform.position);

            Debug.Log("Boom!");
            if(distanceToPlayer <= damageRange)
            {
                Debug.Log("You lost 45HP");
                playerHealth.TakeDamage(explosionDamage);
            }
            Destroy(this.gameObject);
            

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //timer 
        if (enemyDetection.detected && other.gameObject.tag == "Player")
        {
            Debug.Log("triggered");
            explosionMode = true;

        }
    }
}
