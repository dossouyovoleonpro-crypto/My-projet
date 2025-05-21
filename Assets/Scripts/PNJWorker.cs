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
    private Vector3 targetPosition;
    private ResourceManager resourceManager;
    private DayNightCycle dayNightController;

    private static HashSet<GameObject> occupiedResources = new HashSet<GameObject>();
    private static HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

    void Start()
    {
        resourceManager = ResourceManager.Instance;
        dayNightController = FindFirstObjectByType<DayNightCycle>();

        if (dayNightController == null)
            Debug.LogError("❌ [PNJWorker] DayNightCycle non trouvé !");
        else
            Debug.Log("✅ [PNJWorker] DayNightCycle trouvé.");

        if (obstacleMap == null)
            Debug.LogError("❌ [PNJWorker] obstacleMap non assignée !");

        StartCoroutine(WorkCycle());
    }

    IEnumerator WorkCycle()
    {
        while (true)
        {
            if (!isWorking && dayNightController != null && dayNightController.IsDayTime())
            {
                GameObject targetGO = FindNearestWorkTarget();
                Vector3Int? targetTile = FindNearestTileTarget();

                if (targetGO != null)
                {
                    occupiedResources.Add(targetGO);
                    yield return HandleGameObjectResource(targetGO);
                    occupiedResources.Remove(targetGO);
                }
                else if (targetTile.HasValue)
                {
                    occupiedTiles.Add(targetTile.Value);
                    yield return HandleTileResource(targetTile.Value);
                    occupiedTiles.Remove(targetTile.Value);
                }
                else
                {
                    Debug.Log($"❌ [{gameObject.name}] Aucun élément trouvé à proximité.");
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HandleGameObjectResource(GameObject target)
    {
        targetPosition = target.transform.position;
        Debug.Log($"🎯 [{gameObject.name}] Cible GameObject trouvée : {target.name}");

        yield return MoveToTarget(targetPosition);

        isWorking = true;
        float timer = 0f;
        while (timer < workDuration && target != null)
        {
            transform.position = target.transform.position;
            timer += Time.deltaTime;
            yield return null;
        }

        if (target != null)
        {
            ProcessResource(target);
            Destroy(target);
            Debug.Log($"✅ [{gameObject.name}] Travail terminé sur {target.name}.");
        }

        isWorking = false;
    }

    IEnumerator HandleTileResource(Vector3Int cellPos)
    {
        Vector3 worldPos = obstacleMap.GetCellCenterWorld(cellPos);
        Debug.Log($"🎯 [{gameObject.name}] Cible Tile trouvée à {worldPos}");

        yield return MoveToTarget(worldPos);

        TileBase tile = obstacleMap.GetTile(cellPos);
        if (tile == null)
        {
            Debug.LogWarning($"⚠️ [PNJWorker] Tile inexistante à {cellPos}, abandon immédiat de la récolte !");
            yield break;
        }

        isWorking = true;
        float timer = 0f;
        while (timer < workDuration)
        {
            transform.position = worldPos;
            timer += Time.deltaTime;
            yield return null;
        }

        tile = obstacleMap.GetTile(cellPos);
        if (tile != null)
        {
            SaveManager saveManager = FindFirstObjectByType<SaveManager>();
            if (saveManager != null)
                saveManager.RegisterTileDeletion(cellPos);

            ProcessTile(tile);
            obstacleMap.SetTile(cellPos, null);
            Debug.Log($"✅ [{gameObject.name}] Travail terminé sur la Tile.");
        }
        else
        {
            Debug.LogWarning($"⚠️ [{gameObject.name}] Tile inexistante à {cellPos}, tentative de récolte annulée !");
        }

        isWorking = false;
    }

    void ProcessResource(GameObject target)
    {
        string name = target.name.ToLower();

        if (name.Contains("arbre"))
        {
            resourceManager.AddWood(20);
            Debug.Log($"🌲 [{gameObject.name}] Arbre récolté. +20 Bois.");
        }
        else if (name.Contains("baie"))
        {
            resourceManager.AddFood(20);
            Debug.Log($"🍓 [{gameObject.name}] Baie récoltée. +20 Nourriture.");
        }
        else if (name.Contains("caillou"))
        {
            resourceManager.AddStone(20);
            Debug.Log($"🪨 [{gameObject.name}] Pierre extraite. +20 Pierre.");
        }
    }

    void ProcessTile(TileBase tile)
    {
        string tileName = tile.name.ToLower();

        if (tileName.Contains("0111"))
        {
            resourceManager.AddWood(20);
            Debug.Log($"🌲 [{gameObject.name}] Arbre Tile récolté. +20 Bois.");
        }
        else if (tileName.Contains("berry"))
        {
            resourceManager.AddFood(20);
            Debug.Log($"🍓 [{gameObject.name}] Baie Tile récoltée. +20 Nourriture.");
        }
        else if (tileName.Contains("mountain_landscape"))
        {
            resourceManager.AddStone(20);
            Debug.Log($"🪨 [{gameObject.name}] Pierre Tile extraite. +20 Pierre.");
        }
    }

    GameObject FindNearestWorkTarget()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var obj in allObjects)
        {
            if (obj.CompareTag("Ghost")) continue;
            if (occupiedResources.Contains(obj)) continue;

            string name = obj.name.ToLower();
            if (name.Contains("arbre") || name.Contains("baie") || name.Contains("caillou"))
            {
                float dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist < minDistance && dist <= detectionRange)
                {
                    minDistance = dist;
                    nearest = obj;
                }
            }
        }

        return nearest;
    }

    Vector3Int? FindNearestTileTarget()
    {
        Vector3Int nearestCell = new Vector3Int();
        float minDistance = Mathf.Infinity;
        bool found = false;

        foreach (var pos in obstacleMap.cellBounds.allPositionsWithin)
        {
            if (occupiedTiles.Contains(pos)) continue;

            TileBase tile = obstacleMap.GetTile(pos);
            if (tile != null)
            {
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
        }

        if (!found)
            Debug.LogWarning($"❌ [{gameObject.name}] Aucune Tile naturelle trouvée à proximité.");

        return found ? nearestCell : (Vector3Int?)null;
    }

    IEnumerator MoveToTarget(Vector3 destination)
    {
        Debug.Log($"🚶 [{gameObject.name}] En route vers {destination}");

        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * 2f);
            yield return null;
        }

        Debug.Log($"📍 [{gameObject.name}] Arrivé à destination.");
    }
}
