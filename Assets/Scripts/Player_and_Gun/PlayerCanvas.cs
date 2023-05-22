using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCanvas : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text dollarUI;

    [SerializeField] private UnitHealth health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health.CalculateHealth();

        dollarUI.text = "$ " + health.currency.ToString();
    }
}
