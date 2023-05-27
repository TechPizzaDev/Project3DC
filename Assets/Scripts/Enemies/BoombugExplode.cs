using UnityEngine;
using UnityEngine.VFX;

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
        //Counts down till enemy explod
        if (explosionMode)
        {
            timer -= Time.deltaTime;
        }

        //Enemy explodes
        if (timer <= 0)
        {
            //Checks the distance of the player, Needed to see if player should take damage or not
            Transform target = enemyDetection.targetTransform;
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            Debug.Log("Boom!");

            //Spawns an explotion where the enemy was
            Instantiate(explosionFX, transform.position, Quaternion.identity);
            
            //Checks if the player is in range for the expotion to take damage
            if (distanceToPlayer <= damageRange)
            {
                if (target.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(explosionDamage);
                }
            }

            unitHealth.DestroyObj();
        }
    }

    /// <summary>
    /// Triggers when player is close enough to start expotion
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (enemyDetection.detected && other.transform == enemyDetection.targetTransform)
        {
            explosionMode = true;
        }
    }
}
