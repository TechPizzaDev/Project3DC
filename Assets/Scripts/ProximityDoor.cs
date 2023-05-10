using UnityEngine;

public class ProximityDoor : MonoBehaviour
{
    public float Speed = 5f;
    public float ProximityRadius = 10;
    public float Expanse = 2.5f;

    public GameObject LeftDoor;
    public GameObject RightDoor;

    public Transform targetTransform;

    private Vector3 leftStartPos;
    private Vector3 leftEndPos;
    private Vector3 rightStartPos;
    private Vector3 rightEndPos;

    // Start is called before the first frame update
    void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Quaternion rotation = transform.rotation;
        leftStartPos = LeftDoor.transform.position;
        leftEndPos = leftStartPos + rotation * new Vector3(0, 0, Expanse);
        rightStartPos = RightDoor.transform.position;
        rightEndPos = rightStartPos - rotation * new Vector3(0, 0, Expanse);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 leftPos = LeftDoor.transform.position;
        Vector3 rightPos = RightDoor.transform.position;

        if (Vector3.Distance(targetTransform.position, transform.position) <= ProximityRadius)
        {
            leftPos = Vector3.Lerp(leftPos, leftEndPos, Time.deltaTime * Speed);
            rightPos = Vector3.Lerp(rightPos, rightEndPos, Time.deltaTime * Speed);
        }
        else
        {
            leftPos = Vector3.Lerp(leftPos, leftStartPos, Time.deltaTime * Speed);
            rightPos = Vector3.Lerp(rightPos, rightStartPos, Time.deltaTime * Speed);
        }

        LeftDoor.transform.position = leftPos;
        RightDoor.transform.position = rightPos;
    }
}
