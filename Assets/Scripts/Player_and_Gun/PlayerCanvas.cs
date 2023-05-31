using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProceduralRooms;
using System.Linq;
using System.Collections.Generic;
using System;

public class PlayerCanvas : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text dollarUI;
    public TMP_Text enemyCounterTxt;
    public TMP_Text levelStateTxt;

    [SerializeField] private UnitHealth health;

    public RoomScript currentRoom;
    public RoomGenerator roomGenerator;

    private List<RoomScript> rooms;

    // Start is called before the first frame update
    void Start()
    {
        RoomScript.OnClose += RoomScript_OnClose;
        RoomScript.OnOpen += RoomScript_OnOpen;

        enemyCounterTxt.enabled = false;
        levelStateTxt.text = "";
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

        if (roomGenerator != null)
        {
            var roomState = roomGenerator.RoomState;
            if (rooms == null)
            {
                RefreshRooms(roomState);
            }

            int enemiesRemaining = rooms.Select(room => room.GetExpectedEnemyCount()).Sum();
            int totalEnemiesToSlay = rooms.Select(room => room.GetMaxEnemyCount()).Sum();
            float percent = 1f - (enemiesRemaining / (float)totalEnemiesToSlay);

            levelStateTxt.text = $"{enemiesRemaining} / {totalEnemiesToSlay}; {(int)MathF.Round(percent * 100)}%";
        }
    }

    private void RefreshRooms(RoomGeneratorState roomState)
    {
        rooms = roomState.SpawnedRooms.Values
            .OfType<RoomGenItem>()
            .Select(x => x.GetUniqueComponent<RoomScript>())
            .Where(x => x != null).ToList();
    }
}
