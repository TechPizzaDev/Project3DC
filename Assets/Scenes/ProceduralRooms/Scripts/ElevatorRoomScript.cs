using UnityEngine;

namespace ProceduralRooms
{
    public class ElevatorRoomScript : MonoBehaviour
    {
        private ProximityDoor[] _doors;

        public GameObject RootObject;

        public bool IsOpen = true;

        void Awake()
        {
            if (RootObject == null)
            {
                RootObject = gameObject.transform.parent.gameObject;
            }

            _doors = RootObject.GetComponentsInChildren<ProximityDoor>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OpenRoom()
        {
            foreach (ProximityDoor door in _doors)
            {
                door.ForcedState = ProximityDoorState.Open;
                door.UpdateMaterial();
            }

            IsOpen = true;
        }

        public void CloseRoom()
        {
            foreach (ProximityDoor door in _doors)
            {
                door.ForcedState = ProximityDoorState.Closed;
                door.UpdateMaterial();
            }

            IsOpen = false;
        }
    }
}
