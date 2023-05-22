using UnityEngine;

namespace ProceduralRooms
{
    public class ProximityDoor : MonoBehaviour
    {
        private const float DoorOpenDelay = 0.5f;

        public float Speed = 5f;
        public float ProximityRadius = 10;
        public float Expanse = 2.5f;

        public RoomDoorDirection Direction;
        public GameObject LeftDoor;
        public GameObject RightDoor;

        public Transform TargetTransform;
        public AudioClip OpenClip;
        public AudioClip CloseClip;

        private AudioSource audioSource;

        private Vector3 leftStartPos;
        private Vector3 leftEndPos;
        private Vector3 rightStartPos;
        private Vector3 rightEndPos;

        private float openTime;
        private float closedTime;
        private bool isOpen;
        private float openPause;

        // Start is called before the first frame update
        void Start()
        {
            if (TargetTransform == null)
            {
                TargetTransform = GameObject.FindGameObjectWithTag("Player").transform;
            }

            audioSource = GetComponent<AudioSource>();

            Quaternion rotation = transform.rotation;
            leftStartPos = LeftDoor.transform.position;
            leftEndPos = leftStartPos + rotation * new Vector3(0, 0, Expanse);
            rightStartPos = RightDoor.transform.position;
            rightEndPos = rightStartPos - rotation * new Vector3(0, 0, Expanse);

            openTime = float.Epsilon;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 leftPos = LeftDoor.transform.position;
            Vector3 rightPos = RightDoor.transform.position;
            float delta = Time.deltaTime;

            openPause += delta;
            if (openPause > DoorOpenDelay)
            {
                isOpen = Vector3.Distance(TargetTransform.position, transform.position) <= ProximityRadius;
            }

            if (isOpen)
            {
                leftPos = Vector3.Lerp(leftPos, leftEndPos, delta * Speed);
                rightPos = Vector3.Lerp(rightPos, rightEndPos, delta * Speed);

                if (closedTime == 0) // The door has just closed
                {
                    audioSource.clip = CloseClip;
                    audioSource.Play();
                    openPause = 0;
                }
                closedTime += delta;
                openTime = 0;
            }
            else
            {
                leftPos = Vector3.Lerp(leftPos, leftStartPos, delta * Speed);
                rightPos = Vector3.Lerp(rightPos, rightStartPos, delta * Speed);

                if (openTime == 0) // The door has just opened
                {
                    audioSource.clip = OpenClip;
                    audioSource.Play();
                    openPause = 0;
                }
                openTime += delta;
                closedTime = 0;
            }

            LeftDoor.transform.position = leftPos;
            RightDoor.transform.position = rightPos;
        }
    }
}