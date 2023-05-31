using System.Linq;
using UnityEngine;

namespace ProceduralRooms
{
    public class RoomGenItem : GeneratorItem
    {
        public RoomGenItem(Vector3Int position, RoomDoorDirection openDoors) : base(position, openDoors)
        {
        }

        public override void Generate(RoomGeneratorState state)
        {
            base.Generate(state);

            Vector3Int roomPosition = Position * state.GetRoomSizeVector();
            GameObject roomObj = state.InstantiateRoom(state.Root, roomPosition, OpenDoors);
            GeneratedObjects.Add(roomObj);

            ProximityDoor[] doors = roomObj.GetComponentsInChildren<ProximityDoor>();

            for (int i = 0; i < 4; i++)
            {
                RoomDoorDirection door = (RoomDoorDirection)(1 << i);
                if ((OpenDoors & door) == 0)
                {
                    doors.First(x => x.Direction == door).enabled = false;
                    continue;
                }

                Vector3Int newPosition = Position + GetVector(door);
                if (state.SpawnedRooms.ContainsKey(newPosition) || state.SpawnedRooms.Count >= 100)
                {
                    // Carry on, we already have this room.
                    continue;
                }

                Vector3 tunnelVec = GetVector(door);
                Vector3Int roomSize = state.GetRoomSizeVector();
                Vector3 tunnelCenter = roomPosition + new Vector3(
                    roomSize.x * tunnelVec.x / 2f,
                    0,
                    roomSize.z * tunnelVec.z / 2f);
                GameObject tunnelObj = state.InstantiateTunnel(state.Root, tunnelCenter, door);
                GeneratedObjects.Add(tunnelObj);

                RoomDoorDirection extraDoors = RoomDoorDirection.None;
                for (int j = 0; j < 4; j++)
                {
                    RoomDoorDirection nestedDoor = (RoomDoorDirection)(1 << j);
                    if (state.Rng.Next(3) == 0)
                    {
                        if (!state.SpawnedRooms.ContainsKey(newPosition + GetVector(nestedDoor)))
                        {
                            extraDoors |= nestedDoor;
                        }
                    }
                }

                RoomGenItem newRoom = new(newPosition, OppositeDirection(door) | extraDoors);
                state.ItemStack.Push(newRoom);
            }
        }
    }
}
