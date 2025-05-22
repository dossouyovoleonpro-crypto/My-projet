using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public GameObject pnjPrefab;
    public LayerMask buildingLayer;

    private GameObject selectedPrefab;
    private bool deleteMode = false;
    private bool isPlacingPrefab = false;

    private Camera mainCamera;
    private GameObject ghostInstance;
    public Material ghostMaterial;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleGhostMovement();

        if (isPlacingPrefab && !deleteMode && Input.GetMouseButtonDown(0))
        {
            if (IsClickOnUIButton())
            {
                Debug.Log("🛑 Clic sur un bouton UI détecté, pas de placement.");
                return;
            }

            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            PlacePrefab(mousePosition);
        }

        if (deleteMode && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, buildingLayer);

            if (hit.collider != null)
            {
                GameObject target = hit.collider.gameObject;

                if (target.CompareTag("Building") || target.CompareTag("Feu") || target.CompareTag("Mairie"))
                {
                    Debug.Log($"🗑️ Suppression de l'objet : {target.name}");

                    if (target.name.ToLower().Contains("maison"))
                        ResourceManager.Instance.RemovePopulation(3);

                    if (target.name.ToLower().Contains("foyer"))
                        ResourceManager.Instance.RemovePopulation(5);

                    Destroy(target);
                }
                else
                {
                    Debug.Log("❌ L'objet cliqué n'est pas un bâtiment valide pour la suppression.");
                }
            }
            else
            {
                Debug.Log("❌ Aucun objet détecté sous le clic pour la suppression.");
            }
        }
    }

    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
        isPlacingPrefab = true;

        if (ghostInstance != null)
            Destroy(ghostInstance);

        ghostInstance = Instantiate(selectedPrefab);
        ghostInstance.tag = "Ghost";

        ApplyGhostVisual(ghostInstance);
    }

    public void DeselectPrefab()
    {
        selectedPrefab = null;
        isPlacingPrefab = false;

        if (ghostInstance != null)
            Destroy(ghostInstance);
    }

    public GameObject GetSelectedPrefab()
    {
        return selectedPrefab;
    }

    public bool HasSelectedPrefab()
    {
        return selectedPrefab != null && isPlacingPrefab;
    }

    public void SetDeleteMode(bool value)
    {
        deleteMode = value;
        if (value) DeselectPrefab();
    }

    public bool IsDeleteMode()
    {
        return deleteMode;
    }

    public void PlacePrefab(Vector2 position)
    {
        if (selectedPrefab != null)
        {
            if (selectedPrefab.name.Contains("Feu") && GameObject.FindGameObjectWithTag("Feu") != null)
            {
                Debug.Log("❌ Un seul Feu est autorisé !");
                return;
            }

            if (selectedPrefab.name.Contains("Mairie") && GameObject.FindGameObjectWithTag("Mairie") != null)
            {
                Debug.Log("❌ Une seule Mairie est autorisée !");
                return;
            }

            BuildingCost cost = selectedPrefab.GetComponent<BuildingCost>();
            if (cost != null && ResourceManager.Instance.HasEnoughResources(cost))
            {
                Vector3 placePos = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), 0f);
                PlacementOffset offset = selectedPrefab.GetComponent<PlacementOffset>();
                if (offset != null)
                    placePos = offset.GetOffsetPosition(placePos);

                // 🔒 Vérifie que la position est libre (avec marge)
                if (!PlacementValidator.IsPositionClear(placePos, GridPlacer2D.Instance.terrainMap, GridPlacer2D.Instance.obstacleMap))
                {
                    Debug.Log("❌ Emplacement invalide : déjà occupé.");
                    return;
                }

                GameObject newObj = Instantiate(selectedPrefab, placePos, Quaternion.identity);

                if (selectedPrefab.name.Contains("Feu"))
                    newObj.tag = "Feu";
                else if (selectedPrefab.name.Contains("Mairie"))
                    newObj.tag = "Mairie";
                else
                    newObj.tag = "Building";

                newObj.layer = LayerMask.NameToLayer("Building");

                if (newObj.GetComponent<Collider2D>() == null)
                {
                    newObj.AddComponent<BoxCollider2D>();
                    Debug.Log($"🧩 BoxCollider2D ajouté automatiquement à {newObj.name}");
                }

                BuildingIdentifier identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = selectedPrefab.name;

                Debug.Log($"🏗️ Bâtiment placé : {selectedPrefab.name} à {placePos}");
                ResourceManager.Instance.SpendResources(cost);

                if (selectedPrefab.name.ToLower().Contains("maison"))
                {
                    ResourceManager.Instance.AddPopulation(3);
                    SpawnPNJsAround(placePos, 3, newObj.transform);
                }
                if (selectedPrefab.name.ToLower().Contains("foyer"))
                {
                    ResourceManager.Instance.AddPopulation(5);
                    SpawnPNJsAround(placePos, 5, newObj.transform);
                }
            }
            else
            {
                Debug.Log("❌ Pas assez de ressources pour construire !");
            }
        }
    }

    public void SpawnPNJsAround(Vector3 center, int count, Transform parent = null)
    {
        if (pnjPrefab == null)
        {
            Debug.LogWarning("❌ Aucun prefab de PNJ assigné dans BuildManager !");
            return;
        }

        Vector2[] directions = {
            Vector2.up * 3f, Vector2.down * 3f,
            Vector2.left * 3f, Vector2.right * 3f,
            new Vector2(1f, 1f).normalized * 3f,
            new Vector2(-1f, 1f).normalized * 3f,
            new Vector2(1f, -1f).normalized * 3f,
            new Vector2(-1f, -1f).normalized * 3f
        };

        int spawned = 0;

        for (int i = 0; i < directions.Length && spawned < count; i++)
        {
            Vector3 spawnPos = center + (Vector3)directions[i];
            GameObject pnj = Instantiate(pnjPrefab, spawnPos, Quaternion.identity);

            if (parent != null)
                pnj.transform.SetParent(parent);

            spawned++;
        }

        if (spawned < count)
        {
            Debug.LogWarning($"⚠️ Seuls {spawned}/{count} PNJ ont été placés autour de {center}.");
        }
    }

    private void ApplyGhostVisual(GameObject ghost)
    {
        SpriteRenderer[] renderers = ghost.GetComponentsInChildren<SpriteRenderer>();

        foreach (var renderer in renderers)
        {
            if (ghostMaterial != null)
                renderer.material = ghostMaterial;

            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }

        foreach (var script in ghost.GetComponents<MonoBehaviour>())
            script.enabled = false;
    }

    private bool IsClickOnUIButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);

            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Button>() != null)
                    return true;
            }
        }
        return false;
    }

    private void HandleGhostMovement()
    {
        if (isPlacingPrefab && !deleteMode && ghostInstance != null)
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 ghostPos = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0f);

            PlacementOffset offset = selectedPrefab.GetComponent<PlacementOffset>();
            if (offset != null)
                ghostPos = offset.GetOffsetPosition(ghostPos);

            ghostInstance.transform.position = ghostPos;
        }
    }
}
