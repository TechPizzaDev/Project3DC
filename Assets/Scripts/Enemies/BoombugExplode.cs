using UnityEngine;

public class BoombugExplode : MonoBehaviour
{
    public int explosionDamage = 45;
    public float damageRange = 3;
    
    EnemyDetection enemyDetection;

    [SerializeField] float timer = 1f;
    public bool explosionMode;

    void Start()
    {
        enemyDetection = GetComponent<EnemyDetection>();
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
            if (distanceToPlayer <= damageRange)
            {
                if (target.TryGetComponent(out UnitHealth health))
                {
                    health.DealDamage(explosionDamage);
                }
            }
            Destroy(gameObject);
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
