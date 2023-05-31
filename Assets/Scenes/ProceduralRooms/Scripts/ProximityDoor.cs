using UnityEngine;

namespace ProceduralRooms
{
    public class ProximityDoor : MonoBehaviour
    {
        private const float DoorOpenDelay = 0.5f;
        private const float ForcedStateSpeedFactor = 2f;

        public float Speed = 5f;
        public float ProximityRadius = 10;
        public float Expanse = 2.5f;

        public RoomDoorDirection Direction;
        public ProximityDoorState ForcedState;

        public GameObject LeftDoor;
        public GameObject RightDoor;
        public GameObject Frame;

        public Transform TargetTransform;

        public Material NormalFrameMaterial;
        public Material ForcedFrameMaterial;

        public AudioClip OpenClip;
        public AudioClip CloseClip;

        private AudioSource audioSource;
        private MeshRenderer frameMeshRenderer;

        private Vector3 leftStartPos;
        private Vector3 leftEndPos;
        private Vector3 rightStartPos;
        private Vector3 rightEndPos;

        private float openTime;
        private float closedTime;
        private ProximityDoorState state = ProximityDoorState.Closed;
        private float openPause;

        // Start is called before the first frame update
        void Start()
        {
            if (TargetTransform == null)
            {
                TargetTransform = GameObject.FindGameObjectWithTag("Player").transform;
            }

            audioSource = GetComponent<AudioSource>();
            frameMeshRenderer = Frame.GetComponent<MeshRenderer>();

            Quaternion rotation = transform.rotation;

            if (LeftDoor != null)
                leftStartPos = LeftDoor.transform.position;
            leftEndPos = leftStartPos + rotation * new Vector3(0, 0, Expanse);

            if (RightDoor != null)
                rightStartPos = RightDoor.transform.position;
            rightEndPos = rightStartPos - rotation * new Vector3(0, 0, Expanse);

            // prevent audio playing on start
            openTime = float.Epsilon;
            closedTime = float.Epsilon;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 leftPos = LeftDoor != null ? LeftDoor.transform.position : default;
            Vector3 rightPos = RightDoor != null ? RightDoor.transform.position : default;
            float delta = Time.deltaTime;

            openPause += delta;
            float speed = delta * Speed;

            if (ForcedState != ProximityDoorState.Unset)
            {
                state = ForcedState;
                speed *= ForcedStateSpeedFactor; // close faster when forced
            }
            else if (openPause > DoorOpenDelay)
            {
                if (Vector3.Distance(TargetTransform.position, transform.position) <= ProximityRadius)
                {
                    state = ProximityDoorState.Open;
                }
                else
                {
                    state = ProximityDoorState.Closed;
                }
            }

            if (state == ProximityDoorState.Open)
            {
                leftPos = Vector3.Lerp(leftPos, leftEndPos, speed);
                rightPos = Vector3.Lerp(rightPos, rightEndPos, speed);

                if (openTime == 0) // The door has started opening
                {
                    UpdateMaterial();

                    audioSource.clip = OpenClip;
                    audioSource.Play();
                    openPause = 0;
                }
                openTime += delta;
                closedTime = 0;
            }
            else
            {
                leftPos = Vector3.Lerp(leftPos, leftStartPos, speed);
                rightPos = Vector3.Lerp(rightPos, rightStartPos, speed);

                if (closedTime == 0) // The door has started closing
                {
                    UpdateMaterial();

                    audioSource.clip = CloseClip;
                    audioSource.Play();
                    openPause = 0;
                }
                closedTime += delta;
                openTime = 0;
            }

            if (LeftDoor != null)
                LeftDoor.transform.position = leftPos;
            if (RightDoor != null)
                RightDoor.transform.position = rightPos;
        }

        public void UpdateMaterial()
        {
            if (ForcedState != ProximityDoorState.Unset)
            {
                frameMeshRenderer.sharedMaterial = ForcedFrameMaterial;
            }
            else
            {
                frameMeshRenderer.sharedMaterial = NormalFrameMaterial;
            }
        }
    }
}