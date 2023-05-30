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
            state.InstantiateElevator(state.Root, roomPosition);

            Vector3Int tunnelVec = GetVector(RoomDoorDirection.Forward);
            Vector3Int roomSize = state.GetRoomSizeVector();
            Vector3 tunnelCenter = roomPosition + new Vector3(
                roomSize.x * tunnelVec.x / 2f,
                0,
                roomSize.z * tunnelVec.z / 2f);
            state.InstantiateTunnel(state.Root, tunnelCenter, RoomDoorDirection.Forward);

            state.ItemStack.Push(new RoomGenItem(tunnelVec, RoomDoorDirection.All));
        }
    }
}
