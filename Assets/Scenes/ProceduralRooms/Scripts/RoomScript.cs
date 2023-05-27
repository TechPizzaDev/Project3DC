using System;
using ProceduralRooms;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public static event Action<RoomScript> OnOpen;
    public static event Action<RoomScript> OnClose;

    private MobSpawner[] _mobSpawners;
    private ProximityDoor[] _doors;

    public GameObject EntranceColliderGroup;
    public GameObject RootObject;

    public bool IsOpen = true;

    void Awake()
    {
        if (RootObject == null)
        {
            RootObject = gameObject.transform.parent.gameObject;
        }

        _mobSpawners = RootObject.GetComponentsInChildren<MobSpawner>();
        _doors = RootObject.GetComponentsInChildren<ProximityDoor>();
    }

    private void Update()
    {
        if (!IsOpen)
        {
            int currentCount = GetCurrentEnemyCount();
            if (currentCount == 0)
            {
                OpenRoom();
            }
        }
    }

    public int GetCurrentEnemyCount()
    {
        int count = 0;
        foreach (MobSpawner spawner in _mobSpawners)
        {
            count += spawner.CurrentMobCount;
        }
        return count;
    }

    public int GetMaxEnemyCount()
    {
        int count = 0;
        foreach (MobSpawner spawner in _mobSpawners)
        {
            count += spawner.SpawnCount;
        }
        return count;
    }

    public void OpenRoom()
    {
        foreach (ProximityDoor door in _doors)
        {
            door.ForcedState = ProximityDoorState.Open;
        }

        IsOpen = true;
        OnOpen?.Invoke(this);
    }

    public void CloseRoom()
    {
        EntranceColliderGroup.SetActive(false);

        foreach (ProximityDoor door in _doors)
        {
            door.ForcedState = ProximityDoorState.Closed;
        }

        foreach (MobSpawner spawner in _mobSpawners)
        {
            spawner.StartSpawning();
        }

        IsOpen = false;
        OnClose?.Invoke(this);
    }

    public void OnEntranceTrigger(EntranceTrigger.Event ev)
    {
        if (!IsOpen)
        {
            return;
        }

        if (_mobSpawners.Length > 0)
        {
            CloseRoom();
        }
    }
}
