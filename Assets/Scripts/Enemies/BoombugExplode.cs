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
        if (explosionMode)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            Transform target = enemyDetection.targetTransform;
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            Debug.Log("Boom!");

            Instantiate(explosionFX, transform.position, Quaternion.identity); //Quaternion.identity = no rotation
            

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

    private void OnTriggerEnter(Collider other)
    {
        if (enemyDetection.detected && other.transform == enemyDetection.targetTransform)
        {
            explosionMode = true;
        }
    }
}
