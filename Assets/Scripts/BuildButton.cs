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
        // Toujours désactiver le mode suppression quand on clique sur un bouton de construction
        if (BuildManager.Instance.IsDeleteMode())
        {
            BuildManager.Instance.SetDeleteMode(false);
            Debug.Log("Mode suppression désactivé via bouton de construction.");
        }

        BuildingCost cost = prefabToPlace.GetComponent<BuildingCost>();
        if (cost != null && !ResourceManager.Instance.HasEnoughResources(cost))
        {
            Debug.Log("Pas assez de ressources pour construire !");
            return;
        }

        // Activer ou désactiver le mode placement
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
