using UnityEngine;
using UnityEngine.UI;

public class EnemyCanvas : MonoBehaviour
{
    public Slider healthSlider;

    [SerializeField] private UnitHealth health;

    // Start is called before the first frame update
    void Start()
    {
        if (health == null)
        {
            health = GetComponent<UnitHealth>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health.CalculateHealth();

        if (health.CurrentHealth < health.MaxHealth)
        {
            healthSlider.gameObject.SetActive(true);
        }
        else
        {
            healthSlider.gameObject.SetActive(false);
        }
        //healthSlider.gameObject.SetActive(health.damaged);
    }
}
