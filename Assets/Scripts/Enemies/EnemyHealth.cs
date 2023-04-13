using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;
    private float gunDamage = 60;

    [SerializeField] GameObject healthBarUI;
    [SerializeField] Slider healthSlider;

    void Start()
    {
        health = maxHealth;
        healthSlider.value = CalculateHealth();
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
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            health -= gunDamage;
        }
    }
}
