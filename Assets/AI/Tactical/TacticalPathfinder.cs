using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
namespace LOGIYGames
{
    public class TacticalPathfinder : MonoBehaviour
    {
        public GridManager gridManager;
        public float dangerPenalty = 20.0f;

        public List<Node> FindTacticalPath(Node startNode, Node endNode)
        {
            var openList = new PriorityQueue<Node, float>();
            var closedSet = new HashSet<Vector3>();
            var cameFrom = new Dictionary<Vector3, Vector3>();
            var costSoFar = new Dictionary<Vector3, float> { [startNode.position] = 0 };
            var exposureSoFar = new Dictionary<Vector3, float> { [startNode.position] = 0 };

            openList.Enqueue(startNode, 0);

            while (openList.Count > 0)
            {
                Node currentNode = openList.Dequeue();

                if (currentNode.position == endNode.position)
                    return ReconstructPath(cameFrom, startNode, endNode);

                closedSet.Add(currentNode.position);

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor.position)) continue;

                    float exposure = exposureSoFar[currentNode.position];
                    int dangerScore = gridManager.CalculateNodeDanger(gridManager.GetSectors(neighbor.position));

                    float tempExposure = exposure;
                    float newCost = costSoFar[currentNode.position] + TacticalCostNodeToNode(currentNode, neighbor, ref tempExposure, dangerScore);
                    if (costSoFar.TryGetValue(neighbor.position, out float existingCost) && newCost >= existingCost) continue;

                    costSoFar[neighbor.position] = newCost;
                    exposureSoFar[neighbor.position] = tempExposure;
                    cameFrom[neighbor.position] = currentNode.position;

                    float priority = newCost + Heuristic(neighbor, endNode);
                    openList.Enqueue(neighbor, priority);

                    // DisplayTacticalCost(neighbor.position, newCost); 
                }
            }

            Debug.LogWarning("No path found!");
            return null;
        }

        float Heuristic(Node fromNode, Node toNode)
        {
            float baseHeuristic = fromNode.position.SquaredDistance(toNode.position);
            int dangerScore = gridManager.CalculateNodeDanger(gridManager.GetSectors(toNode.position));
            float heuristicPenalty = dangerScore * dangerPenalty;
            return baseHeuristic + heuristicPenalty;
        }

        float TacticalCostNodeToNode(Node fromNode, Node toNode, ref float cumulativeExposure, int dangerScore)
        {
            float travelTime = Vector3.Distance(fromNode.position, toNode.position);

            if (dangerScore > 0)
            {
                cumulativeExposure += dangerScore * dangerPenalty;
            }

            float exposureRefund = 0;
            if (dangerScore == 0)
            {
                exposureRefund = -cumulativeExposure;
                cumulativeExposure = 0;
            }

            float penalty = Mathf.Pow(3, dangerScore);
            return travelTime + penalty * penalty + exposureRefund;
        }

        IEnumerable<Node> GetNeighbors(Node node)
        {
            Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            return directions
                .Select(direction => gridManager.GetNodeFromWorldPosition(node.position + direction * gridManager.cellSize))
                .Where(neighbor => neighbor.isWalkable);
        }

        List<Node> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Node startNode, Node endNode)
        {
            var path = new List<Node>();
            Vector3 current = endNode.position;

            while (current != startNode.position)
            {
                path.Add(new Node(current));
                current = cameFrom[current];
            }

            path.Add(startNode);
            path.Reverse();
            return SimplifyPath(path);
        }

        List<Node> SimplifyPath(List<Node> fullPath)
        {
            if (fullPath == null || fullPath.Count < 2) return fullPath;

            var simplifiedPath = new List<Node> { fullPath[0] };

            for (int i = 1; i < fullPath.Count - 1; i++)
            {
                if (gridManager.CalculateNodeDanger(gridManager.GetSectors(fullPath[i].position)) == 0)
                {
                    simplifiedPath.Add(fullPath[i]);
                }
            }

            simplifiedPath.Add(fullPath[^1]);
            return simplifiedPath;
        }

        void DisplayTacticalCost(Vector3 position, float cost)
        {
            GameObject costTextObj = new GameObject("CostText");
            costTextObj.transform.parent = gridManager.transform;
            costTextObj.transform.position = position + new Vector3(0, 1.5f, 0); // Raise text above the ground
            TextMeshPro textMesh = costTextObj.AddComponent<TextMeshPro>();
            textMesh.text = cost.ToString("F1"); // Display cost with one decimal
            textMesh.fontSize = 2;
            textMesh.alignment = TextAlignmentOptions.Center;

            if (cost < 10) textMesh.color = Color.green;
            else if (cost < 20) textMesh.color = Color.yellow;
            else if (cost < 40) textMesh.color = Color.red;
            else textMesh.color = new Color(0.5f, 0, 0); // Dark red for extreme danger

            Destroy(costTextObj, 15.0f);
        }
    }
}