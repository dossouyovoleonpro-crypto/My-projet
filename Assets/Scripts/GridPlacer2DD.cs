using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridPlacer2D : MonoBehaviour
{
    public Tilemap terrainMap;
    public Tilemap obstacleMap;
    public LayerMask clickableLayer;

    private Dictionary<string, bool> uniquePlaced = new Dictionary<string, bool>
    {
        { "Feu", false },
        { "Mairie", false }
    };

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = terrainMap.WorldToCell(mousePos);
            Vector3 worldAlignedPos = terrainMap.GetCellCenterWorld(cellPos);

            GameObject selectedPrefab = BuildManager.Instance.GetSelectedPrefab();
            if (selectedPrefab == null) return;

            if (!terrainMap.HasTile(cellPos) || obstacleMap.HasTile(cellPos))
            {
                Debug.Log("❌ Impossible de placer ici : pas de terrain ou obstacle présent.");
                return;
            }

            if (BuildManager.Instance.IsDeleteMode())
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, clickableLayer);
                if (hit.collider != null)
                {
                    foreach (var key in uniquePlaced.Keys)
                    {
                        if (hit.collider.gameObject.name.Contains(key))
                        {
                            uniquePlaced[key] = false;
                            break;
                        }
                    }

                    Destroy(hit.collider.gameObject);
                    return;
                }
            }
            else
            {
                foreach (var key in uniquePlaced.Keys)
                {
                    if (selectedPrefab.name.Contains(key) && uniquePlaced[key])
                    {
                        Debug.Log("⚠️ Un seul " + key + " est autorisé.");
                        return;
                    }
                }

                // Instanciation et paramétrage du bâtiment
                GameObject newObj = Instantiate(selectedPrefab, worldAlignedPos, Quaternion.identity);
                newObj.tag = "Building"; // ✅ Important pour la sauvegarde

                // ✅ Ajout du BuildingIdentifier pour mémoriser le prefab d'origine
                BuildingIdentifier identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = selectedPrefab.name;

                Debug.Log($"🏗️ Objet placé : {selectedPrefab.name} à {worldAlignedPos}");

                foreach (var key in uniquePlaced.Keys)
                {
                    if (selectedPrefab.name.Contains(key))
                    {
                        uniquePlaced[key] = true;
                        break;
                    }
                }
            }
        }
    }
}
