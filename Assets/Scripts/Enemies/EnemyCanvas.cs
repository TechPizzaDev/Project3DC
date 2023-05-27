using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class manages the enemy's health UI canvas.
/// It updates the health slider based on the enemy's health value.
/// </summary>
/// 
public class EnemyCanvas : MonoBehaviour
{
    public Slider healthSlider;

    [SerializeField] private UnitHealth health;

    void Start()
    {  
        // If the health component is not assigned, get the health component from the same game object
        if (health == null)
        {
            health = GetComponent<UnitHealth>();
        }
    }

    void Update()
    {
        // Update the health slider value based on the enemy's health
        healthSlider.value = health.CalculateHealth();

        // Set the health slider game object active or inactive based on the enemy's health
        if (health.CurrentHealth < health.MaxHealth)
        {
            healthSlider.gameObject.SetActive(true);
        }
        else
        {
            healthSlider.gameObject.SetActive(false);
        }
    }
}
