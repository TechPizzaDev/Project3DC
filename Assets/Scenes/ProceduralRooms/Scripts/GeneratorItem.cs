using System;
using UnityEngine;

namespace ProceduralRooms
{
    public abstract class GeneratorItem
    {
        public Vector3Int Position { get; }
        public RoomDoorDirection OpenDoors { get; }

        protected GeneratorItem(Vector3Int position, RoomDoorDirection openDoors)
        {
            Position = position;
            OpenDoors = openDoors;
        }

        public virtual void Generate(RoomGeneratorState state)
        {
            state.SpawnedRooms.Add(Position, this);
        }

        public static Vector3Int GetVector(RoomDoorDirection direction)
        {
            return direction switch
            {
                RoomDoorDirection.Left => new Vector3Int(-1, 0, 0),
                RoomDoorDirection.Right => new Vector3Int(1, 0, 0),
                RoomDoorDirection.Forward => new Vector3Int(0, 0, 1),
                RoomDoorDirection.Backward => new Vector3Int(0, 0, -1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        public static RoomDoorDirection OppositeDirection(RoomDoorDirection direction)
        {
            return direction switch
            {
                RoomDoorDirection.Left => RoomDoorDirection.Right,
                RoomDoorDirection.Right => RoomDoorDirection.Left,
                RoomDoorDirection.Forward => RoomDoorDirection.Backward,
                RoomDoorDirection.Backward => RoomDoorDirection.Forward,
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }
    }
}
