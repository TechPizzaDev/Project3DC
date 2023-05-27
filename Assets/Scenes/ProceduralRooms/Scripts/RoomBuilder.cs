using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace ProceduralRooms
{
    public class RoomBuilder : MonoBehaviour
    {
        public int RoomWidth = 25;
        public int RoomHeight = 10;
        public int RoomDepth = 25;

        public int DoorWidth = 5;
        public int DoorHeight = 6;

        public Material FloorMaterial;

        // Start is called before the first frame update
        void Start()
        {
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public GameObject BuildRoom(RoomDoorDirection openDoors)
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
            if ((openDoors & RoomDoorDirection.Left) != 0)
            {
                CarveDoor(RoomDepth, RoomHeight, wallNX);
            }

            ProBuilderMesh wallPX = GenerateWall(RoomDepth, RoomHeight, new Vector3(1, 0, 0));
            wallPX.transform.position = new Vector3(RoomWidth, RoomHeight, 0) / 2;
            wallPX.transform.parent = room.transform;
            if ((openDoors & RoomDoorDirection.Right) != 0)
            {
                CarveDoor(RoomDepth, RoomHeight, wallPX);
            }

            ProBuilderMesh wallNZ = GenerateWall(RoomWidth, RoomHeight, new Vector3(0, 0, -1));
            wallNZ.transform.position = new Vector3(0, RoomHeight, -RoomDepth) / 2;
            wallNZ.transform.parent = room.transform;
            if ((openDoors & RoomDoorDirection.Backward) != 0)
            {
                CarveDoor(RoomWidth, RoomHeight, wallNZ);
            }

            ProBuilderMesh wallPZ = GenerateWall(RoomWidth, RoomHeight, new Vector3(0, 0, 1));
            wallPZ.transform.position = new Vector3(0, RoomHeight, RoomDepth) / 2;
            wallPZ.transform.parent = room.transform;
            if ((openDoors & RoomDoorDirection.Forward) != 0)
            {
                CarveDoor(RoomWidth, RoomHeight, wallPZ);
            }

            return room;
        }

        public void CarveDoor(int width, int height, ProBuilderMesh mesh)
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


        public ProBuilderMesh GenerateFloor(int width, int depth)
        {
            ProBuilderMesh mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, depth, width - 1, depth - 1, Axis.Up);
            mesh.name = "Floor";
            mesh.gameObject.AddComponent<MeshCollider>();
            mesh.GetComponent<MeshRenderer>().material = FloorMaterial;
            return mesh;
        }

        public ProBuilderMesh GenerateCeiling(int width, int depth)
        {
            ProBuilderMesh mesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, depth, width - 1, depth - 1, Axis.Down);
            mesh.name = "Ceiling";
            mesh.gameObject.AddComponent<MeshCollider>();
            mesh.GetComponent<MeshRenderer>().material = FloorMaterial;
            return mesh;
        }

        public ProBuilderMesh GenerateWall(int width, int height, Vector3 axis)
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
    