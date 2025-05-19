using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class PNJWorker : MonoBehaviour
{
    public float workDuration = 10f;
    public float detectionRange = 20f;
    public Tilemap obstacleMap;

    private bool isWorking = false;
    private static HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

    void Start()
    {
        StartCoroutine(WorkCycle());
    }

    IEnumerator WorkCycle()
    {
        while (true)
        {
            if (!isWorking)
            {
                Vector3Int? targetTile = FindNearestTileTarget();

                if (targetTile.HasValue)
                {
                    TileBase tile = obstacleMap.GetTile(targetTile.Value);

                    if (tile != null)
                    {
                        occupiedTiles.Add(targetTile.Value);
                        yield return HandleTileResource(targetTile.Value);
                        occupiedTiles.Remove(targetTile.Value);
                    }
                    else
                    {
                        Debug.Log($"🚫 [PNJWorker] Abandon de la case {targetTile.Value}, plus de ressource présente avant déplacement.");
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HandleTileResource(Vector3Int cellPos)
    {
        Vector3 worldPos = obstacleMap.GetCellCenterWorld(cellPos);

        yield return MoveToTarget(worldPos);

        // ✅ Vérification critique juste avant de commencer le travail
        TileBase tile = obstacleMap.GetTile(cellPos);
        if (tile == null)
        {
            Debug.LogWarning($"⚠️ [PNJWorker] Tile inexistante à {cellPos}, abandon immédiat de la récolte !");
            yield break; // Arrêt immédiat de la coroutine
        }

        isWorking = true;

        float timer = 0f;
        while (timer < workDuration)
        {
            transform.position = worldPos;
            timer += Time.deltaTime;
            yield return null;
        }

        // Vérification finale avant la suppression
        tile = obstacleMap.GetTile(cellPos);
        if (tile != null)
        {
            SaveManager saveManager = FindFirstObjectByType<SaveManager>();
            if (saveManager != null)
            {
                saveManager.RegisterTileDeletion(cellPos);
            }

            obstacleMap.SetTile(cellPos, null);
            Debug.Log($"✅ [PNJWorker] Tile supprimée à {cellPos}");
        }
        else
        {
            Debug.LogWarning($"⚠️ [PNJWorker] Tile inexistante à {cellPos}, tentative de récolte annulée !");
        }

        isWorking = false;
    }

    Vector3Int? FindNearestTileTarget()
    {
        SaveManager saveManager = FindFirstObjectByType<SaveManager>();

        Vector3Int nearestCell = new Vector3Int();
        float minDistance = Mathf.Infinity;
        bool found = false;

        foreach (var pos in obstacleMap.cellBounds.allPositionsWithin)
        {
            if (occupiedTiles.Contains(pos)) continue;
            if (saveManager != null && saveManager.IsTileDeleted(pos)) continue;

            TileBase tile = obstacleMap.GetTile(pos);
            if (tile == null) continue;

            string tileName = tile.name.ToLower();
            if (tileName.Contains("0111") || tileName.Contains("berry") || tileName.Contains("mountain_landscape"))
            {
                Vector3 worldPos = obstacleMap.GetCellCenterWorld(pos);
                float dist = Vector3.Distance(transform.position, worldPos);
                if (dist < minDistance && dist <= detectionRange)
                {
                    minDistance = dist;
                    nearestCell = pos;
                    found = true;
                }
            }
        }

        return found ? nearestCell : (Vector3Int?)null;
    }

    IEnumerator MoveToTarget(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 2f);
            yield return null;
        }
    }
}
