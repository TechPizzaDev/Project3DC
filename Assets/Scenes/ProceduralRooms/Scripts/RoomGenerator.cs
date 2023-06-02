using System.Collections.Generic;
using System.Linq;
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

        public RoomGeneratorState RoomState { get; private set; }

        NavMeshData _navMesh;
        NavMeshDataInstance _navInstance;

        void OnEnable()
        {
            _navInstance = NavMesh.AddNavMeshData(_navMesh);
        }

        void OnDisable()
        {
            // Unload navmesh and clear handle
            _navInstance.Remove();
        }

        void Awake()
        {
            _navMesh = new NavMeshData();

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

            // Enqueue the first generator item.
            state.ItemStack.Push(new ElevatorGenItem(Vector3Int.zero));

            while (state.ItemStack.TryPop(out GeneratorItem generatorItem))
            {
                generatorItem.Generate(state);
            }

            // Register all elevators after generation.
            foreach (ElevatorGenItem elevator in state.SpawnedRooms.Values.OfType<ElevatorGenItem>())
            {
                var elevatorScript = elevator.GetUniqueComponent<ElevatorRoomScript>();
                RoomScript.OnClose += (room) => elevatorScript.CloseRoom();
            }

            // Generate navmesh by collecting all generated attached to the RoomGeneratorState.Root transform.
            var sources = new List<NavMeshBuildSource>();
            NavMeshBuilder.CollectSources(state.Root, NavLayerMask.value, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);

            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            defaultBuildSettings.agentRadius = 1;

            var bounds = new Bounds(new Vector3(0, -10, 0), new Vector3(10000, 100, 10000));
            NavMeshBuilder.UpdateNavMeshData(_navMesh, defaultBuildSettings, sources, bounds);

            RoomState = state;
        }
    }
}
