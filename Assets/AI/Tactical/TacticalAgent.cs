using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace LOGIYGames
{
    public class TacticalAgent : MonoBehaviour
    {
        public GridManager gridManager;
        public TacticalPathfinder pathfinder;
        public Transform destination;

        NavMeshAgent navMeshAgent;
        List<Vector3> waypoints;
        int currentWaypointIndex;

        public Vector3 GetMovementVelocity() => navMeshAgent.velocity;

        void Start()
        {
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            SetCustomPath();
        }

        void Update()
        {
            FollowCustomPath();
        }

        void SetCustomPath()
        {
            Node startNode = gridManager.GetNodeFromWorldPosition(transform.position);
            Node endNode = gridManager.GetNodeFromWorldPosition(destination.position);
            List<Node> path = pathfinder.FindTacticalPath(startNode, endNode);

            if (path?.Count > 0)
            {
                waypoints = path.ConvertAll(node => node.position + new Vector3(0.5f, 0, 0.5f));
                currentWaypointIndex = 0;
            }
        }

        void FollowCustomPath()
        {
            if (waypoints == null || currentWaypointIndex >= waypoints.Count)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                return;
            }

            navMeshAgent.SetDestination(waypoints[currentWaypointIndex]);
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 1f)
            {
                currentWaypointIndex++;
            }
        }
    }
}