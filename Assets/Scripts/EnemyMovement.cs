using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 startingPos;
    [SerializeField] private Vector2 endPos;
    [SerializeField] private Vector2 walkingToPos;
    private float margin = 1;
    bool destination = false;

    void Start()
    {
        startingPos = transform.position;
        speed = 1;

        endPos = new Vector2 (0,5);
        walkingToPos = endPos;

    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.x < walkingToPos.x + margin &&
            transform.position.x > walkingToPos.x - margin &&
            transform.position.z < walkingToPos.y + margin &&
            transform.position.z > walkingToPos.y - margin)
        {
            if (destination)
            {
                walkingToPos = startingPos;
                destination = false;
            }
            else
            {
                walkingToPos = endPos;
                destination = true;
            }
            
        }


        Vector2 v = new Vector2(transform.position.x, transform.position.z);
        direction = walkingToPos - v;
        direction = direction.normalized;
        Vector3 movementDirection = new Vector3(direction.x, 0, direction.y);
        transform.position += movementDirection * speed * Time.deltaTime; 

    }
}
