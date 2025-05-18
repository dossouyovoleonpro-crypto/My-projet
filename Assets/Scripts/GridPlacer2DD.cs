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
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = terrainMap.WorldToCell(mousePos);
            Vector3 worldAlignedPos = terrainMap.GetCellCenterWorld(cellPos);

            GameObject selectedPrefab = BuildManager.Instance.GetSelectedPrefab();
            if (selectedPrefab == null) return;

            // Vérification du placement valide
            if (!terrainMap.HasTile(cellPos) || obstacleMap.HasTile(cellPos))
            {
                Debug.Log("Impossible de placer ici : pas de terrain ou obstacle présent.");
                return;
            }

            if (BuildManager.Instance.IsDeleteMode())
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, clickableLayer);
                if (hit.collider != null)
                {
                    GameObject hitObject = hit.collider.gameObject;

                    // Vérifie si c'est une maison avant de la supprimer
                    if (hitObject.name.Contains("Maison"))
                    {
                        ResourceManager.Instance.RemovePopulation(3);
                    }

                    // Vérifie et met à jour les bâtiments uniques
                    foreach (var key in uniquePlaced.Keys)
                    {
                        if (hitObject.name.Contains(key))
                        {
                            uniquePlaced[key] = false;
                            break;
                        }
                    }

                    Destroy(hitObject);
                    return;
                }
            }
            else
            {
                // Vérification des bâtiments uniques (Feu, Mairie)
                foreach (var key in uniquePlaced.Keys)
                {
                    if (selectedPrefab.name.Contains(key) && uniquePlaced[key])
                    {
                        Debug.Log("Un seul " + key + " est autorisé.");
                        return;
                    }
                }

                // Instanciation du bâtiment
                GameObject newObj = Instantiate(selectedPrefab, worldAlignedPos, Quaternion.identity);

                // Ajout de l’identifiant pour la sauvegarde
                BuildingIdentifier identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = selectedPrefab.name;

                Debug.Log("Objet placé : " + selectedPrefab.name);

                // Mise à jour des bâtiments uniques
                foreach (var key in uniquePlaced.Keys)
                {
                    if (selectedPrefab.name.Contains(key))
                    {
                        uniquePlaced[key] = true;
                        break;
                    }
                }

                // Si c'est une maison, on ajoute 3 à la population
                if (selectedPrefab.name.ToLower().Contains("maison"))
                {
                    ResourceManager.Instance.AddPopulation(3);
                }
            }
        }
    }
}
