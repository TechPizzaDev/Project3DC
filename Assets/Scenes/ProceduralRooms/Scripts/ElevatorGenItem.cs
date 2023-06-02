using System.Linq;
using UnityEngine;

namespace ProceduralRooms
{
    public class ElevatorGenItem : GeneratorItem
    {
        public ElevatorGenItem(Vector3Int position) : base(position, RoomDoorDirection.Forward)
        {
        }

        public override void Generate(RoomGeneratorState state)
        {
            base.Generate(state);

            Vector3Int roomPosition = Position * state.GetRoomSizeVector();
            GeneratedObjects.Add(state.InstantiateElevator(state.Root, roomPosition));

            Vector3Int tunnelVec = GetVector(RoomDoorDirection.Forward);
            Vector3Int roomSize = state.GetRoomSizeVector();
            Vector3 tunnelCenter = roomPosition + new Vector3(
                roomSize.x * tunnelVec.x / 2f,
                0,
                roomSize.z * tunnelVec.z / 2f);
            GeneratedObjects.Add(state.InstantiateTunnel(state.Root, tunnelCenter, RoomDoorDirection.Forward));

            // Make the room in front of the elevator room have doors open in all directions.
            state.ItemStack.Push(new RoomGenItem(tunnelVec, RoomDoorDirection.All));
        }
    }
}
