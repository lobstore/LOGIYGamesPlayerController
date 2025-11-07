using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotWalker : MonoBehaviour
{
    NavMeshAgent agent;
    
    [SerializeField] Transform dest;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(dest.position);
    }

    private void Update()
    {
        
    }

}
