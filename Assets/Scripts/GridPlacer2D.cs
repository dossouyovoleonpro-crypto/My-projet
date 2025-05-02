using UnityEngine;
using UnityEngine.EventSystems;

public class GridPlacer2D : MonoBehaviour
{
    public LayerMask clickableLayer;

    private bool feuDeCampPlace = false;

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, clickableLayer);

            if (hit.collider == null) return;

            if (BuildManager.Instance.IsDeleteMode())
            {
                // Si on supprime un feu de camp, réactive le droit d’en poser un
                if (hit.collider.gameObject.name.Contains("Feu"))
                {
                    feuDeCampPlace = false;
                    Debug.Log("Feu de camp supprimé.");
                }

                Destroy(hit.collider.gameObject);
                Debug.Log("Objet supprimé : " + hit.collider.name);
            }
            else
            {
                GameObject prefab = BuildManager.Instance.GetSelectedPrefab();
                if (prefab == null) return;

                // Blocage uniquement pour le feu de camp
                if (prefab.name.Contains("Feu") && feuDeCampPlace)
                {
                    Debug.Log("Un seul feu de camp est autorisé.");
                    return;
                }

                Vector2 spawnPos = new Vector2(
                    Mathf.Floor(mousePos.x) + 0.5f,
                    Mathf.Floor(mousePos.y) + 0.5f
                );

                Instantiate(prefab, spawnPos, Quaternion.identity);
                Debug.Log("Prefab placé : " + prefab.name);

                // Marquer le feu de camp comme placé
                if (prefab.name.Contains("Feu"))
                    feuDeCampPlace = true;
            }
        }
    }
}