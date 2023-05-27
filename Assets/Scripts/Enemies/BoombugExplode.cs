using UnityEngine;
using UnityEngine.VFX;


/// <summary>
/// This class manages the explosion behavior of a Boombug enemy.
/// It handles triggering the explosion, damaging nearby objects, and destroying the enemy itself.
/// </summary>

public class BoombugExplode : MonoBehaviour
{
    UnitHealth unitHealth;

    [SerializeField] GameObject explosionFX;
    public int explosionDamage = 45;
    public float damageRange = 3;
    
    EnemyDetection enemyDetection;

    [SerializeField] float timer = 1f;
    float explosionFXTimer = 1f;
    public bool explosionMode;

    

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();
        unitHealth = GetComponent<UnitHealth>();
        
    }

    void Update()
    {
        if (explosionMode)
        {
            timer -= Time.deltaTime; // Decrease the timer for the explosion
        }

        if (timer <= 0)
        {
            Transform target = enemyDetection.targetTransform; // Get the target transform from EnemyDetection
            float distanceToPlayer = Vector3.Distance(transform.position, target.position); // Calculate the distance to the player

            Debug.Log("Boom!");

            // Create the explosion visual effect
            Instantiate(explosionFX, transform.position, Quaternion.identity); //Quaternion.identity = no rotation

            if (distanceToPlayer <= damageRange)
            {
                // Damage objects within the damage range
                if (target.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(explosionDamage);
                }
            }

            unitHealth.DestroyObj(); // Destroy the enemy object after the explosion
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the enemy is detected and the entering collider is the target, enable explosion mode
        if (enemyDetection.detected && other.transform == enemyDetection.targetTransform)
        {
            explosionMode = true;
        }
    }
}
