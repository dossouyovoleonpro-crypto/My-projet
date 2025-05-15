using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private GameObject selectedPrefab;
    private bool deleteMode = false;
    private bool isPlacingPrefab = false;

    private Camera mainCamera;

    // Gestion du Ghost (Aperçu du bâtiment)
    private GameObject ghostInstance;
    public Material ghostMaterial; // À assigner dans l'inspecteur

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
        // Déplacement du ghost avec la souris
        if (isPlacingPrefab && !deleteMode && ghostInstance != null)
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 ghostPos = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0f);
            ghostInstance.transform.position = ghostPos;
        }

        // Clic gauche pour placer l'objet
        if (isPlacingPrefab && !deleteMode && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            PlacePrefab(mousePosition);
        }
    }

    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
        isPlacingPrefab = true;

        // Détruire l'ancien ghost si existant
        if (ghostInstance != null)
            Destroy(ghostInstance);

        // Créer le ghost
        ghostInstance = Instantiate(selectedPrefab);
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
            BuildingCost cost = selectedPrefab.GetComponent<BuildingCost>();

            if (cost != null && ResourceManager.Instance.HasEnoughResources(cost))
            {
                Vector3 placePosition = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), 0f);
                Instantiate(selectedPrefab, placePosition, Quaternion.identity);

                ResourceManager.Instance.SpendResources(cost);
            }
            else
            {
                Debug.Log("Pas assez de ressources pour construire !");
            }
        }
    }

    private void ApplyGhostVisual(GameObject ghost)
    {
        SpriteRenderer[] renderers = ghost.GetComponentsInChildren<SpriteRenderer>();

        foreach (var renderer in renderers)
        {
            if (ghostMaterial != null)
            {
                renderer.material = ghostMaterial;
            }

            // Assure la transparence même si aucun matériau n'est défini
            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }

        // Désactive tous les scripts de comportement du prefab pour le ghost
        foreach (var script in ghost.GetComponents<MonoBehaviour>())
        {
            script.enabled = false;
        }
    }
}
