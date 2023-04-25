using System;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Rng = System.Random;

namespace ProceduralRooms
{
    [DefaultExecutionOrder(-1000)]
    public class RoomGenerator : MonoBehaviour
    {
        public int RoomWidth = 25;
        public int RoomHeight = 10;
        public int RoomDepth = 25;

        public int DoorWidth = 5;
        public int DoorHeight = 6;

        public int RoomMargin = 1;
        public int Seed = 1234;

        public Material FloorMaterial;

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

        enum DoorDirection
        {
            None = 0,
            Left = 1 << 0,
            Right = 1 << 1,
            Forward = 1 << 2,
            Backward = 1 << 3,

            All = Left | Right | Forward | Backward,
        }

        class RoomGen
        {
            public Vector3Int Position;
            public DoorDirection OpenDoors;
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
                Position = new Vector3Int(),
                OpenDoors = DoorDirection.Forward,
            });
            spawnedRooms.Add(roomGens.Peek().Position, roomGens.Peek());

            roomGens.Push(new RoomGen()
            {
                Position = GetVector(DoorDirection.Forward),
                OpenDoors = DoorDirection.All,
            });
            spawnedRooms.Add(roomGens.Peek().Position, roomGens.Peek());

            while (roomGens.TryPop(out RoomGen roomGen))
            {
                GameObject roomObj = GenerateRoom(roomGen.OpenDoors);
                roomObj.transform.position = roomGen.Position * new Vector3Int(RoomWidth + RoomMargin, 0, RoomDepth + RoomMargin);
                roomObj.transform.parent = transform;

                for (int i = 0; i < 4; i++)
                {
                    DoorDirection door = (DoorDirection)(1 << i);
                    if ((roomGen.OpenDoors & door) == 0)
                    {
                        continue;
                    }

                    Vector3Int newPosition = roomGen.Position + GetVector(door);
                    if (!spawnedRooms.ContainsKey(newPosition) && spawnedRooms.Count < 300)
                    {
                        DoorDirection extraDoors = DoorDirection.None;
                        for (int j = 0; j < 4; j++)
                        {
                            DoorDirection nestedDoor = (DoorDirection)(1 << j);
                            if (_rng.Next(2) == 0)
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
            }

            //var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            //var bounds = mesh.GetComponent<MeshRenderer>().bounds;
            //
            //var sources = new List<NavMeshBuildSource>();
            //Collect(new List<MeshFilter>() { mesh.GetComponent<MeshFilter>() }, sources);
            //
            //bounds.Expand(new Vector3(0, 2, 0));
            //NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, sources, bounds);
        }

        private static Vector3Int GetVector(DoorDirection direction)
        {
            return direction switch
            {
                DoorDirection.Left => new Vector3Int(-1, 0, 0),
                DoorDirection.Right => new Vector3Int(1, 0, 0),
                DoorDirection.Forward => new Vector3Int(0, 0, 1),
                DoorDirection.Backward => new Vector3Int(0, 0, -1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        private static DoorDirection OppositeDirection(DoorDirection direction)
        {
            return direction switch
            {
                DoorDirection.Left => DoorDirection.Right,
                DoorDirection.Right => DoorDirection.Left,
                DoorDirection.Forward => DoorDirection.Backward,
                DoorDirection.Backward => DoorDirection.Forward,
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        public static void Collect(List<MeshFilter> m_Meshes, List<NavMeshBuildSource> sources)
        {
            for (int i = 0; i < m_Meshes.Count; ++i)
            {
                MeshFilter mf = m_Meshes[i];
                if (mf == null) continue;

                Mesh m = mf.sharedMesh;
                if (m == null) continue;

                NavMeshBuildSource s = new NavMeshBuildSource();
                s.shape = NavMeshBuildSourceShape.Mesh;
                s.sourceObject = m;
                s.transform = mf.transform.localToWorldMatrix;
                s.area = 0;
                sources.Add(s);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private GameObject GenerateRoom(DoorDirection openDoors)
        {
            GameObject room = new("Room");

            ProBuilderMesh floor = GenerateFloor(RoomWidth, RoomDepth);
            floor.transform.parent = room.transform;

            ProBuilderMesh ceiling = GenerateCeiling(RoomWidth, RoomDepth);
            ceiling.transform.position = new Vector3(0, RoomHeight, 0);
            ceiling.transform.parent = room.transform;

            ProBuilderMesh wallNX = GenerateWall(RoomDepth, RoomHeight, new Vector3(-1, 0, 0));
            wallNX.transform.position = new Vector3(-RoomWidth, RoomHeight, 0) / 2;
            wallNX.transform.parent = room.transform;
            if ((openDoors & DoorDirection.Left) != 0)
            {
                CarveDoor(RoomDepth, RoomHeight, wallNX);
            }

            ProBuilderMesh wallPX = GenerateWall(RoomDepth, RoomHeight, new Vector3(1, 0, 0));
            wallPX.transform.position = new Vector3(RoomWidth, RoomHeight, 0) / 2;
            wallPX.transform.parent = room.transform;
            if ((openDoors & DoorDirection.Right) != 0)
            {
                CarveDoor(RoomDepth, RoomHeight, wallPX);
            }

            ProBuilderMesh wallNZ = GenerateWall(RoomWidth, RoomHeight, new Vector3(0, 0, -1));
            wallNZ.transform.position = new Vector3(0, RoomHeight, -RoomDepth) / 2;
            wallNZ.transform.parent = room.transform;
            if ((openDoors & DoorDirection.Backward) != 0)
            {
                CarveDoor(RoomWidth, RoomHeight, wallNZ);
            }

            ProBuilderMesh wallPZ = GenerateWall(RoomWidth, RoomHeight, new Vector3(0, 0, 1));
            wallPZ.transform.position = new Vector3(0, RoomHeight, RoomDepth) / 2;
            wallPZ.transform.parent = room.transform;
            if ((openDoors & DoorDirection.Forward) != 0)
            {
                CarveDoor(RoomWidth, RoomHeight, wallPZ);
            }

            return room;
        }

        private void CarveDoor(int width, int height, ProBuilderMesh mesh)
        {
            List<int> faceIndices = new(width * height);

            for (int y = 0; y < DoorHeight; y++)
            {
                for (int x = 0; x < DoorWidth; x++)
                {
                    int dx = x + width / 2 - DoorWidth / 2;
                    faceIndices.Add(y * width + dx);
                }
            }

            mesh.DeleteFaces(faceIndices);
            mesh.ToMesh();
            mesh.Refresh();
        }


        private ProBuilderMesh GenerateFloor(int width, int depth)
        {
            ProBuilderMesh mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, depth, width - 1, depth - 1, Axis.Up);
            mesh.name = "Floor";
            mesh.gameObject.AddComponent<MeshCollider>();
            mesh.gameObject.AddComponent<NavMeshSurface>();
            mesh.GetComponent<MeshRenderer>().material = FloorMaterial;
            return mesh;
        }

        private ProBuilderMesh GenerateCeiling(int width, int depth)
        {
            ProBuilderMesh mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, depth, width - 1, depth - 1, Axis.Down);
            mesh.name = "Ceiling";
            mesh.gameObject.AddComponent<MeshCollider>();
            mesh.GetComponent<MeshRenderer>().material = FloorMaterial;
            return mesh;
        }

        private ProBuilderMesh GenerateWall(int width, int height, Vector3 axis)
        {
            ProBuilderMesh mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, height, width - 1, height - 1, Axis.Forward);
            mesh.name = "Wall " + axis;
            mesh.transform.rotation = Quaternion.LookRotation(axis) * Quaternion.Euler(0, 180, 0);
            mesh.gameObject.AddComponent<MeshCollider>();
            mesh.GetComponent<MeshRenderer>().material = FloorMaterial;
            return mesh;
        }
    }
}
    