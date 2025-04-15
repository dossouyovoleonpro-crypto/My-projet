using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private GameObject selectedPrefab;
    private bool deleteMode = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

    public void DeselectPrefab()
    {
        selectedPrefab = null;
    }

    public GameObject GetSelectedPrefab()
    {
        return selectedPrefab;
    }

    public bool HasSelectedPrefab()
    {
        return selectedPrefab != null;
    }

    public void SetDeleteMode(bool value)
    {
        deleteMode = value;
        
        if (value)
            DeselectPrefab(); // on désélectionne le prefab si on passe en mode suppression
    }

    public bool IsDeleteMode()
    {
        return deleteMode;
    }
}
