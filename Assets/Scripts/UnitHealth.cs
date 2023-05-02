using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;

    public bool damaged;
    public bool killed;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
    }

    public float CalculateHealth()
    {
        return health / maxHealth;
    }

    public void DealDamage(float damage)
    {
        damaged = true;

        health -= damage;

        if (health <= 0)
        {
            killed = true;
        }
    }
}

