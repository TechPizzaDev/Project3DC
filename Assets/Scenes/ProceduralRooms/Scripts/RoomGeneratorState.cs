using System;
using System.Collections.Generic;
using UnityEngine;
using Rng = System.Random;

namespace ProceduralRooms
{
    public class RoomGeneratorState
    {
        public Transform Root { get; }
        public Rng Rng { get; }

        public Stack<GeneratorItem> ItemStack { get; }
        public Dictionary<Vector3Int, GeneratorItem> SpawnedRooms { get; }

        public int RoomWidth;
        public int RoomDepth;
        public int RoomMargin;

        public GameObject[] Elevators;
        public GameObject[] Rooms;
        public GameObject[] Tunnels;

        public RoomGeneratorState(Transform root, int seed)
        {
            Root = root != null ? root : throw new ArgumentNullException(nameof(root));
            Rng = new Rng(seed);

            ItemStack = new Stack<GeneratorItem>();
            SpawnedRooms = new Dictionary<Vector3Int, GeneratorItem>();
        }

        public Vector3Int GetRoomSizeVector()
        {
            return new Vector3Int(
                RoomWidth + RoomMargin,
                0,
                RoomDepth + RoomMargin);
        }

        public GameObject InstantiateElevator(Transform parent, Vector3 position)
        {
            GameObject prefab = Elevators[Rng.Next(Elevators.Length)];
            GameObject instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, parent);
            return instance;
        }

        public GameObject InstantiateRoom(Transform parent, Vector3 position, RoomDoorDirection openDoors)
        {
            GameObject prefab = Rooms[Rng.Next(Rooms.Length)];
            GameObject instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, parent);
            return instance;
        }

        public GameObject InstantiateTunnel(Transform parent, Vector3 position, RoomDoorDirection direction)
        {
            GameObject prefab = Tunnels[Rng.Next(Tunnels.Length)];
            GameObject instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, parent);
            switch (direction)
            {
                case RoomDoorDirection.Forward:
                    break;

                case RoomDoorDirection.Left:
                case RoomDoorDirection.Right:
                case RoomDoorDirection.Backward:
                    instance.transform.rotation = Quaternion.LookRotation(RoomGenItem.GetVector(direction)) * Quaternion.Euler(0, 180, 0);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
            return instance;
        }
    }
}
