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
        if (BuildManager.Instance.IsDeleteMode())  // Si le mode suppression est activé
        {
            // Si le mode suppression est actif, on ne sélectionne pas de prefab
            Debug.Log("Mode suppression activé, ne peut pas sélectionner le chemin.");
        }
        else
        {
            // Sinon on sélectionne le prefab pour le placement
            if (BuildManager.Instance.GetSelectedPrefab() == prefabToPlace)
            {
                // Si le prefab est déjà sélectionné, on désélectionne
                BuildManager.Instance.DeselectPrefab();
                Debug.Log("Placement désactivé.");
            }
            else
            {
                // Sélectionne le prefab à placer
                BuildManager.Instance.SelectPrefab(prefabToPlace);
                Debug.Log("Prefab sélectionné : " + prefabToPlace.name);
            }
        }
    }
}
