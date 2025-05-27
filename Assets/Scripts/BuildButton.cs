using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    public GameObject prefabToPlace;
    public ResourceInfoDisplay resourceInfoDisplay; // Ajoute cette référence

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (BuildManager.Instance.IsDeleteMode())
        {
            BuildManager.Instance.SetDeleteMode(false);

            DeleteButton deleteButton = Object.FindFirstObjectByType<DeleteButton>();
            if (deleteButton != null)
            {
                deleteButton.ResetButtonColor();
            }

            Debug.Log("Mode suppression désactivé via bouton de construction.");
        }

        BuildingCost costCheck = prefabToPlace.GetComponent<BuildingCost>();

        // Affiche le coût même si les ressources sont insuffisantes
        if (resourceInfoDisplay != null && costCheck != null)
        {
            resourceInfoDisplay.DisplayCost(costCheck); // Nouvelle fonction à ajouter dans ResourceInfoDisplay
        }

        if (costCheck != null && !ResourceManager.Instance.HasEnoughResources(costCheck))
        {
            Debug.Log("Pas assez de ressources pour construire !");
            // ❌ Ne fais pas 'return', on laisse la sélection du prefab pour afficher le coût
        }

        if (BuildManager.Instance.GetSelectedPrefab() == prefabToPlace)
        {
            BuildManager.Instance.DeselectPrefab();
            Debug.Log("Placement désactivé.");
        }
        else
        {
            BuildManager.Instance.SelectPrefab(prefabToPlace);
            Debug.Log("Prefab sélectionné : " + prefabToPlace.name);
        }
    }
}
