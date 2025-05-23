using UnityEngine;
using UnityEngine.Tilemaps;

public static class PlacementValidator
{
    private static readonly string[] forbiddenObstacleKeywords = {
        "0111", "berry", "mountain_landscape", "arbre", "baie", "caillou", "Overworld_38", "Overworld_77", "Overworld_39", "Overworld_40", "Overworld_41", "Overworld_77", "Overworld_158", "Overworld_276"
    };

    public static bool IsPositionClear(Vector3 worldPosition, Tilemap terrainMap, Tilemap obstacleMap)
    {
        Vector2 size = Vector2.one;
        Vector2 offset = Vector2.zero;

        GameObject selectedPrefab = BuildManager.Instance.GetSelectedPrefab();
        if (selectedPrefab != null)
        {
            ColliderSettings settings = selectedPrefab.GetComponent<ColliderSettings>();
            if (settings != null)
            {
                size = settings.customSize;
                offset = settings.customOffset;
            }
        }

        Vector2 testSize = size + new Vector2(0.8f, 0.8f);
        Bounds placementArea = new Bounds(worldPosition + (Vector3)offset, testSize);

        // Vérifie tous les colliders dans la zone
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(placementArea.center, placementArea.size, 0f);
        foreach (var hit in overlaps)
        {
            string name = hit.gameObject.name.ToLower();

            if (hit.CompareTag("Building") || hit.CompareTag("Feu") || hit.CompareTag("Mairie"))
                return false;

            foreach (var keyword in forbiddenObstacleKeywords)
            {
                if (name.Contains(keyword))
                    return false;
            }
        }

        // Vérifie les tiles naturelles
        Vector2 halfSize = testSize / 2f;
        for (float x = -halfSize.x + 0.5f; x < halfSize.x; x++)
        {
            for (float y = -halfSize.y + 0.5f; y < halfSize.y; y++)
            {
                Vector3Int cell = terrainMap.WorldToCell(worldPosition + new Vector3(x, y, 0f));
                TileBase tile = obstacleMap.GetTile(cell);

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

        return true;
    }
}
