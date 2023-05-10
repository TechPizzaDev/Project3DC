using UnityEditor;
using UnityEngine;

namespace ProceduralRooms
{
    [CustomEditor(typeof(RoomBuilder))]
    public class RoomBuilderEditor : Editor
    {
        private RoomDoorDirection _openDoors;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            _openDoors = (RoomDoorDirection)EditorGUILayout.EnumFlagsField("Open doors", _openDoors);

            var builder = (RoomBuilder)target;
            if (GUILayout.Button("Generate"))
            {
                GameObject roomObj = builder.BuildRoom(_openDoors);
                roomObj.transform.position = new Vector3(0, 0, 0);
                roomObj.transform.parent = builder.transform;
            }
        }
    }
}
