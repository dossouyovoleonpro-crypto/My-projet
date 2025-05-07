using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    public GameObject prefabToPlace;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (BuildManager.Instance.IsDeleteMode())
        {
            Debug.Log("Mode suppression activé.");
            return;
        }

        // Empêche la sélection d'un unique déjà placé
        if (GridPlacer2D.Instance != null && GridPlacer2D.Instance.IsUniquePlaced(prefabToPlace.name))
        {
            Debug.Log("Impossible de sélectionner : un seul " + prefabToPlace.name + " autorisé.");
            return;
        }

        BuildingCost cost = prefabToPlace.GetComponent<BuildingCost>();
        if (cost != null && !ResourceManager.Instance.HasEnoughResources(cost))
        {
            Debug.Log("Pas assez de ressources !");
            return;
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
