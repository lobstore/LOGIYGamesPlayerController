using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
namespace LOGIYGames
{
    public struct Node
    {
        public Vector3 position;
        public bool isWalkable;

        public Node(Vector3 position, bool isWalkable = false)
        {
            this.position = position;
            this.isWalkable = isWalkable;
        }
    }

    public class GridManager : MonoBehaviour
    {
        public int gridSizeX = 10;
        public int gridSizeZ = 10;
        public float cellSize = 1.0f;
        public TextMeshPro textPrefab;
        public Material dangerZoneMaterial;

        Node[,] grid;
        // A sector represents an 8-way directional segment (45-degree slices) 
        // around a grid node, used to track enemy influence and danger levels in different directions.
        readonly Dictionary<Vector3, int> nodeSectorData = new(); // Each position will map to an 8-sector bitmask

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x / cellSize);
            int z = Mathf.RoundToInt(worldPosition.z / cellSize);
            x = Mathf.Clamp(x, 0, gridSizeX - 1); // Clamp to grid bounds.
            z = Mathf.Clamp(z, 0, gridSizeZ - 1); // Clamp to grid bounds.
            return grid[x, z];
        }

        void Awake()
        {
            InitializeGrid();
            PrecomputeSectors();
            DrawGrid();
        }

        void DrawGrid()
        {
            for (int x = 0; x < gridSizeX; x++)
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Node node = grid[x, z];
                    int dangerScore = CalculateNodeDanger(GetSectors(node.position));

                    if (dangerScore == 0) continue;

                    GameObject cellVisual = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    Destroy(cellVisual.GetComponent<Collider>());
                    cellVisual.transform.SetParent(transform);
                    cellVisual.transform.SetPositionAndRotation(node.position + new Vector3(cellSize / 2, 0.01f, cellSize / 2), Quaternion.Euler(90, 0, 0));
                    cellVisual.transform.localScale = new Vector3(cellSize, cellSize, 1);
                    cellVisual.GetComponent<Renderer>().sharedMaterial = dangerZoneMaterial;

                    TextMeshPro text3D = Instantiate(textPrefab, node.position + new Vector3(cellSize / 2, 0.1f, cellSize / 2), Quaternion.Euler(90, 0, 0));
                    text3D.text = dangerScore.ToString();
                    text3D.transform.SetParent(transform);
                }
        }

        public int CalculateNodeDanger(int sectorMask)
        {
            int danger = 0;
            for (int sector = 0; sector < 8; sector++)
            {
                danger += (sectorMask >> (sector * 4)) & 0b1111;
            }
            return danger;
        }

        public int GetSectors(Vector3 nodePosition)
        {
            return nodeSectorData.GetValueOrDefault(nodePosition);
        }

        void PrecomputeSectors()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var node in grid)
            {
                if (!node.isWalkable) continue;

                int sectorMask = 0;
                foreach (var enemy in enemies)
                {
                    if (enemy.GetComponent<Enemy>() is not { } enemyComponent) continue;

                    float detectionRadius = enemyComponent.detectionRadius;
                    Vector3 direction = (node.position - enemy.transform.position).normalized;
                    float distance = Vector3.Distance(node.position, enemy.transform.position);

                    if (distance > detectionRadius) continue;
                    if (IsObstructed(enemy.transform.position, node.position + new Vector3(0.5f, 0, 0.5f))) continue;

                    int sector = GetSectorForDirection(direction);
                    int rangeValue = GetRangeValue(distance, detectionRadius);
                    int sectorShift = sector * 4;
                    int currentSectorValue = (sectorMask >> sectorShift) & 0b1111;
                    int newSectorValue = Mathf.Min(15, currentSectorValue + rangeValue);
                    sectorMask &= ~(0b1111 << sectorShift);
                    sectorMask |= newSectorValue << sectorShift;
                }
                nodeSectorData[node.position] = sectorMask;
            }
        }

        int GetRangeValue(float distance, float detectionRadius)
        {
            if (distance < detectionRadius * 0.5f) return 3;
            if (distance < detectionRadius * 0.75f) return 2;
            if (distance <= detectionRadius) return 1;
            return 0;
        }

        int GetSectorForDirection(Vector3 direction)
        {
            return Mathf.FloorToInt((Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg + 360) % 360 / 45f);
        }

        bool IsObstructed(Vector3 from, Vector3 to)
        {
            return Physics.Raycast(from, (to - from).normalized, Vector3.Distance(from, to));
        }

        void InitializeGrid()
        {
            grid = new Node[gridSizeX, gridSizeZ];
            for (int x = 0; x < gridSizeX; x++)
                for (int z = 0; z < gridSizeZ; z++)
                {
                    var pos = new Vector3(x, 0, z) * cellSize;
                    grid[x, z] = new Node(pos, IsPositionOnNavMesh(pos));
                }
        }

        bool IsPositionOnNavMesh(Vector3 position) => NavMesh.SamplePosition(position, out _, cellSize / 2, NavMesh.AllAreas);
    }
}