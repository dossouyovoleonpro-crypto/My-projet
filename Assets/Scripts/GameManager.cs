using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
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
        if (PnjPrefab == null)
        {
            Debug.LogError("❌ [GameManager] PnjPrefab n'est pas assigné !");
            return;
        }

        if (TerrainMap == null || ObstacleMap == null)
        {
            Debug.LogError("❌ [GameManager] Les Tilemaps ne sont pas assignées !");
            return;
        }

        GenerateMap();
        StartCoroutine(DelayedLoadAndSpawn());
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    void GenerateMap()
    {
        Debug.Log("🗺️ [GameManager] Génération de la map terminée.");
        // Place ici la logique de génération de map si elle existe.
    }

    IEnumerator DelayedLoadAndSpawn()
    {
        yield return new WaitForSeconds(0.5f); // ✅ Délai pour laisser le temps à la map d'apparaître

        SaveManager saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
        {
            saveManager.LoadGame();
        }

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

                if (!ObstacleMap.HasTile(cell) && TerrainMap.HasTile(cell))
                {
                    Vector3 worldPos = TerrainMap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0f);
                    validPositions.Add(worldPos);
                }
            }
        }

        if (validPositions.Count == 0)
        {
            Debug.LogWarning("⚠️ [GameManager] Aucune position valide pour le spawn des PNJ.");
            return;
        }

        for (int i = 0; i < pnjCount && validPositions.Count > 0; i++)
        {
            int randIndex = Random.Range(0, validPositions.Count);
            Vector3 spawnPos = validPositions[randIndex];

            if (PnjPrefab != null)
            {
                Instantiate(PnjPrefab, spawnPos, Quaternion.identity);
                Debug.Log($"👤 [GameManager] PNJ instancié à : {spawnPos}");
            }

            validPositions.RemoveAt(randIndex);
        }
    }
}
