using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour// IDamageable
{
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;

    [SerializeField] GameObject healthBarUI;
    [SerializeField] Slider healthSlider;

    public int CurrentHealth { get => (int)health; private set => health = value; }
    public int MaxHealth { get => (int)maxHealth; private set => maxHealth = value;}

    //public event IDamageable.TakeDamageEvent OnTakeDamage;
    //public event IDamageable.DeathEvent OnDeath;

    void Start()
    {
        health = maxHealth;
        //healthSlider.value = CalculateHealth();
    }

    void Update()
    {
        healthSlider.value = CalculateHealth();

        if (health < maxHealth)
        {
            healthBarUI.SetActive(true);
        }
        else if (health == maxHealth)
        {
            healthBarUI.SetActive(false);

        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }

    private float CalculateHealth()
    {
        return health / maxHealth;
    }
    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Bullet")
    //    {
    //        health -= gunDamage;
    //    }
    //}

    //public void TakeDamage(int damage)
    //{
    //    int damageTaken = Mathf.Clamp(damage, 0, CurrentHealth);

    //    CurrentHealth -= damageTaken;

    //    if (damageTaken != 0)
    //    {
    //        OnTakeDamage?.Invoke(damageTaken);
    //    }

    //    if (CurrentHealth == 0 && damageTaken != 0)
    //    {
    //        OnDeath?.Invoke(transform.position);
    //    }
    //}
}
