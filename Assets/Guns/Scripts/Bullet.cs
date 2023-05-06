using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody rigidBody;
    [field: SerializeField]
    public Vector3 spawnLocation
    {
        get;
        private set;
    }
    [SerializeField]
    private float delayedDisableTime = 2f;

    public delegate void CollisionEvent(Bullet bullet, Collision collision);
    public event CollisionEvent OnCollision;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void Spawn(Vector3 spawnForce)
    {
        spawnLocation = transform.position;
        transform.forward = spawnForce.normalized;
        rigidBody.AddForce(spawnForce);
        StartCoroutine(DelayedDisable(delayedDisableTime));
    }

    private IEnumerator DelayedDisable(float time)
    {
        yield return new WaitForSeconds(time);
        OnCollisionEnter(null);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(this, collision);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        OnCollision = null;
    }
}
