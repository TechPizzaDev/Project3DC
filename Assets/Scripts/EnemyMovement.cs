using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 direction;
    
    private Rigidbody rigidBody;
    private Vector3 startingPos;
    private Vector3 endingPos;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        startingPos = transform.position;
        endingPos.x = 5;


    }

    // Update is called once per frame
    void Update()
    {
        speed = 1;
        direction.x = 1;

        transform.Translate(direction * speed * Time.deltaTime);

        if (transform.position.x == endingPos.x)
        {
            direction.x = -1;
            transform.Translate(direction * speed * Time.deltaTime);

        }

    }
}
