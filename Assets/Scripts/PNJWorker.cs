using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class PNJWorker : MonoBehaviour
{
    public float workDuration = 5f;
    public float detectionRange = 40f;
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

        StartCoroutine(WorkCycle());
    }

    IEnumerator WorkCycle()
    {
        while (true)
        {
            PNJAdvenceWorker advanceWorker = GetComponent<PNJAdvenceWorker>();
            if (advanceWorker != null && advanceWorker.IsWorking)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

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
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HandleGameObjectResource(GameObject target)
    {
        targetPosition = target.transform.position;

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
        }

        isWorking = false;
    }

    IEnumerator HandleTileResource(Vector3Int cellPos)
    {
        Vector3 worldPos = obstacleMap.GetCellCenterWorld(cellPos);

        yield return MoveToTarget(worldPos);

        TileBase tile = obstacleMap.GetTile(cellPos);
        if (tile == null)
            yield break;

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
        }

        isWorking = false;
    }

    void ProcessResource(GameObject target)
    {
        string name = target.name.ToLower();

        if (name.Contains("arbre"))
            resourceManager.AddWood(40);
        else if (name.Contains("baie"))
            resourceManager.AddFood(20);
        else if (name.Contains("caillou"))
            resourceManager.AddStone(40);
    }

    void ProcessTile(TileBase tile)
    {
        string tileName = tile.name.ToLower();

        if (tileName.Contains("0111"))
            resourceManager.AddWood(40);
        else if (tileName.Contains("berry"))
            resourceManager.AddFood(20);
        else if (tileName.Contains("mountain_landscape"))
            resourceManager.AddStone(40);
    }

    GameObject FindNearestWorkTarget()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var obj in allObjects)
        {
            if (obj.CompareTag("Ghost") || occupiedResources.Contains(obj)) continue;

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