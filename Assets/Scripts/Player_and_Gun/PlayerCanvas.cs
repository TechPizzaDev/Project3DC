using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCanvas : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text dollarUI;
    public TMP_Text enemyCounterTxt;

    [SerializeField] private UnitHealth health;

    public RoomScript currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        RoomScript.OnClose += RoomScript_OnClose;
        RoomScript.OnOpen += RoomScript_OnOpen;

        enemyCounterTxt.enabled = false;
    }

    private void RoomScript_OnOpen(RoomScript obj)
    {
        currentRoom = null;
        enemyCounterTxt.enabled = false;
    }

    private void RoomScript_OnClose(RoomScript obj)
    {
        currentRoom = obj;
        enemyCounterTxt.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health.CalculateHealth();

        dollarUI.text = "$ " + health.currency.ToString();

        if (currentRoom != null)
        {
            enemyCounterTxt.text = $"E {currentRoom.GetCurrentEnemyCount()} / {currentRoom.GetMaxEnemyCount()}";
        }
    }
}
