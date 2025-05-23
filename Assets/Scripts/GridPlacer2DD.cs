using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridPlacer2D : MonoBehaviour
{
    public static GridPlacer2D Instance;

    public Tilemap terrainMap;
    public Tilemap obstacleMap;
    public LayerMask clickableLayer;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (BuildManager.Instance != null && BuildManager.Instance.HasSelectedPrefab())
        {
            // ✅ Déplacement du ghost sous la souris
            if (!BuildManager.Instance.IsDeleteMode())
            {
                GameObject ghost = BuildManager.Instance.GetGhostInstance();
                if (ghost != null)
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int cellPos = terrainMap.WorldToCell(mousePos);
                    Vector3 worldAlignedPos = terrainMap.GetCellCenterWorld(cellPos);
                    ghost.transform.position = worldAlignedPos;
                }
            }

            // ✅ Placement du bâtiment au clic
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = terrainMap.WorldToCell(mousePos);
                Vector3 worldAlignedPos = terrainMap.GetCellCenterWorld(cellPos);

                GameObject selectedPrefab = BuildManager.Instance.GetSelectedPrefab();

                if (selectedPrefab == null) return;

                // Vérification de placement
                if (!terrainMap.HasTile(cellPos) || obstacleMap.HasTile(cellPos))
                {
                    Debug.Log("⛔ Impossible de placer ici : terrain invalide ou obstacle présent.");
                    return;
                }

                // Vérification des ressources
                BuildingCost cost = selectedPrefab.GetComponent<BuildingCost>();
                if (cost != null && !ResourceManager.Instance.HasEnoughResources(cost))
                {
                    Debug.Log("⛔ Ressources insuffisantes.");
                    return;
                }

                // Placement
                GameObject newObj = Instantiate(selectedPrefab, worldAlignedPos, Quaternion.identity);
                newObj.tag = "Building";

                if (newObj.GetComponent<Collider2D>() == null)
                    newObj.AddComponent<BoxCollider2D>();

                var identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = selectedPrefab.name;

                if (cost != null)
                    ResourceManager.Instance.SpendResources(cost);

                Debug.Log($"✅ {selectedPrefab.name} placé à {worldAlignedPos}");
            }
        }
    }
}
