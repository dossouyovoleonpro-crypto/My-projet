using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private GameObject selectedPrefab;
    private bool deleteMode = false;
    private bool isPlacingPrefab = false; // Indique si on est en train de placer un prefab

    private Camera mainCamera;

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
        mainCamera = Camera.main;  // Pour accéder à la caméra principale
    }

    // Sélectionner un prefab
    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
        isPlacingPrefab = true;  // Activer le mode placement
    }

    // Désélectionner un prefab
    public void DeselectPrefab()
    {
        selectedPrefab = null;
        isPlacingPrefab = false;  // Désactiver le mode placement
    }

    // Nouvelle méthode GetSelectedPrefab pour récupérer le prefab sélectionné
    public GameObject GetSelectedPrefab()
    {
        return selectedPrefab;
    }

    // Vérifie si un prefab est sélectionné (et si on est en mode placement)
    public bool HasSelectedPrefab()
    {
        return selectedPrefab != null && isPlacingPrefab;
    }

    // Passer en mode suppression
    public void SetDeleteMode(bool value)
    {
        deleteMode = value;

        if (value)
            DeselectPrefab(); // Désélectionne le prefab si on passe en mode suppression
    }

    // Vérifie si on est en mode suppression
    public bool IsDeleteMode()
    {
        return deleteMode;
    }

    // Mise à jour du placement en fonction des clics de la souris
    void Update()
    {
        if (isPlacingPrefab && !deleteMode)
        {
            if (Input.GetMouseButtonDown(0)) // Si on clique avec la souris (clic gauche)
            {
                Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); // Convertir la position de la souris en coordonnées du monde 2D
                PlacePrefab(mousePosition); // Placer l'objet à cette position
            }
        }
    }

    // Placer le prefab à la position donnée
    public void PlacePrefab(Vector2 position)
    {
        if (selectedPrefab != null)
        {
            BuildingCost cost = selectedPrefab.GetComponent<BuildingCost>();

            // Vérifie si l'utilisateur a suffisamment de ressources
            if (cost != null && ResourceManager.Instance.HasEnoughResources(cost))
            {
                // Place le prefab à la position donnée dans le monde 2D
                Instantiate(selectedPrefab, position, Quaternion.identity);

                // Déduit les ressources nécessaires pour la construction
                ResourceManager.Instance.SpendResources(cost);
                // **Ne pas désélectionner automatiquement après le placement**
            }
            else
            {
                Debug.Log("Pas assez de ressources pour construire !");
            }
        }
    }
}
