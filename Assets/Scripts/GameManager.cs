using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject PnjPrefab;
    public Tilemap TerrainMap;
    public Tilemap ObstacleMap;
    public Vector2Int GardenBottomLeft;
    public Vector2Int GardenTopRight;
    public int pnjCount = 5;

    void Start()
    {
        SpawnPNJs();
    }

    void SpawnPNJs()
    {
        List<Vector3> validPositions = new List<Vector3>();

        for (int x = GardenBottomLeft.x; x <= GardenTopRight.x; x++)
        {
            for (int y = GardenBottomLeft.y; y <= GardenTopRight.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (!ObstacleMap.HasTile(cell) && TerrainMap.HasTile(cell)) // évite obstacles, reste dans le terrain
                {
                    Vector3 worldPos = TerrainMap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0);
                    validPositions.Add(worldPos);
                }
            }
        }

        for (int i = 0; i < pnjCount && validPositions.Count > 0; i++)
        {
            int randIndex = Random.Range(0, validPositions.Count);
            Vector3 spawnPos = validPositions[randIndex];
            Instantiate(PnjPrefab, spawnPos, Quaternion.identity);
            validPositions.RemoveAt(randIndex); // Évite les doublons
        }
    }
}
