using UnityEngine;

public class UnitHealth : MonoBehaviour, IDamageable
{
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;

    public bool damaged;
    public bool killed;

    EnemyDetection enemyDetection;

    public int CurrentHealth { get => (int)health; private set => health = value; }

    public int MaxHealth { get => (int)maxHealth; private set => maxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    [SerializeField] GameObject dollar;

    void Start()
    {
        health = maxHealth;
        enemyDetection = GetComponent<EnemyDetection>();
    }

    void Update()
    {
    }

    public float CalculateHealth()
    {
        return health / maxHealth;
    }

    //public void DealDamage(float damage)
    //{
    //    damaged = true;

    //    health -= damage;

    //    if (health <= 0)
    //    {
    //        killed = true;
    //    }
    //}

    public void TakeDamage(int damage)
    {
        //Debug.Log("TakeDamage");
        int damageTaken = Mathf.Clamp(damage, 0, CurrentHealth);

        CurrentHealth -= damageTaken;

        if (damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if (CurrentHealth == 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(transform.position);
        }
    }

    public void DestroyObj() 
    { 
        Destroy(gameObject);
        Instantiate(dollar, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity); //Quaternion.identity = no rotation


    }

    public void Detection()
    {
        enemyDetection.detected = true;
    }
}

