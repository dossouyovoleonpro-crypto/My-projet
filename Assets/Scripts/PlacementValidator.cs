using UnityEngine;
using UnityEngine.Tilemaps;

public static class PlacementValidator
{
    private static readonly string[] forbiddenObstacleKeywords = {
        "0111", "berry", "mountain_landscape", "arbre", "baie", "caillou", "mer", "overworld_276"
    };

    public static bool IsPositionClear(Vector3 worldPosition, Tilemap terrainMap, Tilemap obstacleMap)
    {
        GameObject selectedPrefab = BuildManager.Instance.GetSelectedPrefab();
        bool isChemin = false;
        Vector2 size = Vector2.one;
        Vector2 offset = Vector2.zero;

        if (selectedPrefab != null)
        {
            string prefabName = selectedPrefab.name.ToLower();
            if (prefabName.Contains("chemin"))
            {
                isChemin = true;
                size = new Vector2(0.1f, 0.1f);
            }
            else
            {
                ColliderSettings settings = selectedPrefab.GetComponent<ColliderSettings>();
                if (settings != null)
                {
                    size = settings.customSize + new Vector2(0.2f, 0.2f);  // Ajout d'une marge
                    offset = settings.customOffset;
                }
            }
        }

        Bounds placementArea = new Bounds(worldPosition + (Vector3)offset, size);

        Collider2D[] overlaps = Physics2D.OverlapBoxAll(placementArea.center, placementArea.size, 0f);
        foreach (var hit in overlaps)
        {
            if (hit.CompareTag("Building") || hit.CompareTag("Feu") || hit.CompareTag("Mairie"))
                return false;
        }

        Vector2 tileCheckSize = isChemin ? new Vector2(1f, 1f) : size;
        Vector2 halfSize = tileCheckSize / 2f;

        for (float x = -halfSize.x + 0.5f; x < halfSize.x; x++)
        {
            for (float y = -halfSize.y + 0.5f; y < halfSize.y; y++)
            {
                Vector3 checkPos = worldPosition + new Vector3(x, y, 0f);
                Vector3Int cell = terrainMap.WorldToCell(checkPos);

                TileBase[] tiles = { obstacleMap.GetTile(cell), terrainMap.GetTile(cell) };
                foreach (TileBase tile in tiles)
                {
                    if (tile != null)
                    {
                        string tileName = tile.name.ToLower();
                        foreach (string keyword in forbiddenObstacleKeywords)
                        {
                            if (tileName.Contains(keyword))
                                return false;
                        }
                    }
                }
            }
        }

        return true;
    }
}
