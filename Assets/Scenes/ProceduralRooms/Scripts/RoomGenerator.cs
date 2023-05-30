using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        public GameObject[] Elevators;

        public GameObject[] Rooms;

        public GameObject[] Tunnels;

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

        // Start is called before the first frame update
        void Start()
        {
            // TODO: limited aggregation algo?
            RoomGeneratorState state = new(transform, Seed)
            {
                RoomWidth = RoomWidth,
                RoomDepth = RoomDepth,
                RoomMargin = RoomMargin,

                Elevators = Elevators,
                Rooms = Rooms,
                Tunnels = Tunnels
            };

            state.ItemStack.Push(new ElevatorGenItem(Vector3Int.zero));

            while (state.ItemStack.TryPop(out GeneratorItem generatorItem))
            {
                generatorItem.Generate(state);
            }

            var sources = new List<NavMeshBuildSource>();
            NavMeshBuilder.CollectSources(transform, NavLayerMask.value, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);

            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            defaultBuildSettings.agentRadius = 1;

            var bounds = new Bounds(new Vector3(0, -10, 0), new Vector3(10000, 100, 10000));
            NavMeshBuilder.UpdateNavMeshData(_navMesh, defaultBuildSettings, sources, bounds);
        }
    }
}
