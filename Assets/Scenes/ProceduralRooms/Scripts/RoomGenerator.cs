using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Rng = System.Random;

namespace ProceduralRooms
{
    public class RoomGenerator : MonoBehaviour
    {
        public int RoomWidth = 25;
        public int RoomHeight = 10;
        public int RoomDepth = 25;

        public int DoorWidth = 5;
        public int DoorHeight = 6;

        public int RoomMargin = 1;
        public int Seed = 1234;

        public LayerMask NavLayerMask;

        public GameObject[] Rooms;

        public GameObject[] Tunnels;

        Rng _rng;

        NavMeshData _navMesh;
        NavMeshDataInstance _navInstance;

        void OnEnable()
        {
            _navMesh = new NavMeshData();
            _navInstance = NavMesh.AddNavMeshData(_navMesh);
        }

        void OnDisable()
        {
            // Unload navmesh and clear handle
            _navInstance.Remove();
        }

        class RoomGen
        {
            public Vector3Int Position;
            public RoomDoorDirection OpenDoors;
        }

        // Start is called before the first frame update
        void Start()
        {
            // TODO: limited aggregation algo?
            _rng = new Rng(Seed);

            Dictionary<Vector3Int, RoomGen> spawnedRooms = new();

            Stack<RoomGen> roomGens = new();

            roomGens.Push(new RoomGen()
            {
                Position = Vector3Int.zero,
                OpenDoors = RoomDoorDirection.Forward,
            });
            spawnedRooms.Add(roomGens.Peek().Position, roomGens.Peek());

            while (roomGens.TryPop(out RoomGen roomGen))
            {
                GameObject roomObj = InstantiateRoom(roomGen.OpenDoors);
                Vector3Int roomPosition = roomGen.Position * new Vector3Int(RoomWidth + RoomMargin, 0, RoomDepth + RoomMargin);
                roomObj.transform.position = roomPosition;
                roomObj.transform.parent = transform;

                for (int i = 0; i < 4; i++)
                {
                    RoomDoorDirection door = (RoomDoorDirection)(1 << i);
                    if ((roomGen.OpenDoors & door) == 0)
                    {
                        continue;
                    }

                    Vector3Int newPosition = roomGen.Position + GetVector(door);
                    if (spawnedRooms.ContainsKey(newPosition) || spawnedRooms.Count >= 100)
                    {
                        // Carry on, we already have this room.
                        continue;
                    }

                    GameObject tunnelObj = InstantiateTunnel(door);
                    Vector3 tunnelVec = GetVector(door);
                    Vector3 tunnelCenter = roomPosition + new Vector3(
                        (RoomWidth + RoomMargin) * tunnelVec.x / 2f,
                        0,
                        (RoomDepth + RoomMargin) * tunnelVec.z / 2f);

                    tunnelObj.transform.position = tunnelCenter;
                    tunnelObj.transform.parent = transform;

                    RoomDoorDirection extraDoors = RoomDoorDirection.None;
                    for (int j = 0; j < 4; j++)
                    {
                        RoomDoorDirection nestedDoor = (RoomDoorDirection)(1 << j);
                        if (_rng.Next(3) == 0)
                        {
                            if (!spawnedRooms.ContainsKey(newPosition + GetVector(nestedDoor)))
                            {
                                extraDoors |= nestedDoor;
                            }
                        }
                    }

                    RoomGen newRoom = new()
                    {
                        Position = newPosition,
                        OpenDoors = OppositeDirection(door) | extraDoors
                    };
                    roomGens.Push(newRoom);
                    spawnedRooms.Add(newRoom.Position, newRoom);
                }
            }

            var sources = new List<NavMeshBuildSource>();
            NavMeshBuilder.CollectSources(transform, NavLayerMask.value, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);

            var defaultBuildSettings = NavMesh.GetSettingsByID(0);

            var bounds = new Bounds(new Vector3(0, -10, 0), new Vector3(10000, 100, 10000));
            NavMeshBuilder.UpdateNavMeshData(_navMesh, defaultBuildSettings, sources, bounds);
        }

        private static Vector3Int GetVector(RoomDoorDirection direction)
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

        private static RoomDoorDirection OppositeDirection(RoomDoorDirection direction)
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

        // Update is called once per frame
        void Update()
        {
        }

        public GameObject InstantiateRoom(RoomDoorDirection openDoors)
        {
            GameObject prefab = Rooms[_rng.Next(Rooms.Length)];
            GameObject instance = Instantiate(prefab);
            return instance;
        }

        public GameObject InstantiateTunnel(RoomDoorDirection direction)
        {
            GameObject prefab = Tunnels[_rng.Next(Tunnels.Length)];
            GameObject instance = Instantiate(prefab);
            switch (direction)
            {
                case RoomDoorDirection.Forward:
                    break;

                case RoomDoorDirection.Left:
                case RoomDoorDirection.Right:
                case RoomDoorDirection.Backward:
                    instance.transform.rotation = Quaternion.LookRotation(GetVector(direction)) * Quaternion.Euler(0, 180, 0);
                    break;


                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
            return instance;
        }
    }
}
