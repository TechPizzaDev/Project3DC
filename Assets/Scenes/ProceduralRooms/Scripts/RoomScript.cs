using ProceduralRooms;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
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

    public void OpenRoom()
    {
        foreach (ProximityDoor door in _doors)
        {
            door.ForcedState = ProximityDoorState.Open;
        }
    }

    public void CloseRoom()
    {
        EntranceColliderGroup.SetActive(false);

        foreach (ProximityDoor door in _doors)
        {
            door.ForcedState = ProximityDoorState.Closed;
        }
    }

    public void OnEntranceTrigger(EntranceTrigger.Event ev)
    {
        if (!IsOpen)
        {
            return;
        }
        IsOpen = false;

        if (_mobSpawners.Length > 0)
        {
            CloseRoom();
        }
    }
}
