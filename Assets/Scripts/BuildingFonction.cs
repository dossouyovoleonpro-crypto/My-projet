using UnityEngine;

public class BuildingFonction : MonoBehaviour
{
    void OnDisable()
    {
        GameEvents.OnResetGame -= SelfDestruct;
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }
    void OnEnable()
    {
        GameEvents.OnResetGame += SelfDestruct;
    }
}