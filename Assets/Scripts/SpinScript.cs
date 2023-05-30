using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public Vector3 SpinSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(SpinSpeed *  Time.deltaTime);
    }
}
