using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField] private Transform movePosTransform;

    private NavMeshAgent agent;
    void Start()
    {
        agent  = GetComponent<NavMeshAgent>(); 
    }

    void Update()
    {
        agent.destination = movePosTransform.position;  
    }
}
