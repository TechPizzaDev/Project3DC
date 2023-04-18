using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;

    [SerializeField] Slider healthSlider;

    void Start()
    {
        health = maxHealth;
        //healthSlider.value = CalculateHealth();
    }

    void Update()
    {
        healthSlider.value = CalculateHealth();

        if (health <= 0)
        {
            Debug.Log("You died.");
        }

    }

    private float CalculateHealth()
    {
        return health / maxHealth;
    }
}
