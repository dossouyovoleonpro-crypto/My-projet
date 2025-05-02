using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GridPlacer2D : MonoBehaviour
{
    public LayerMask clickableLayer;

    // Dictionnaire pour suivre les objets uniques placés
    private Dictionary<string, bool> uniquePlaced = new Dictionary<string, bool>
    {
        { "Feu", false },
        { "Mairie", false }
    };

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
                foreach (var key in uniquePlaced.Keys)
                {
                    if (hit.collider.gameObject.name.Contains(key))
                    {
                        uniquePlaced[key] = false;
                        Debug.Log(key + " supprimé.");
                        break;
                    }
                }

                Destroy(hit.collider.gameObject);
                Debug.Log("Objet supprimé : " + hit.collider.name);
            }
            else
            {
                GameObject prefab = BuildManager.Instance.GetSelectedPrefab();
                if (prefab == null) return;

                // Vérifie si le prefab est dans les objets uniques
                foreach (var key in uniquePlaced.Keys)
                {
                    if (prefab.name.Contains(key))
                    {
                        if (uniquePlaced[key])
                        {
                            Debug.Log("Un seul " + key + " est autorisé.");
                            return;
                        }
                        break;
                    }
                }

                Vector2 spawnPos = new Vector2(
                    Mathf.Floor(mousePos.x) + 0.5f,
                    Mathf.Floor(mousePos.y) + 0.5f
                );

                Instantiate(prefab, spawnPos, Quaternion.identity);
                Debug.Log("Prefab placé : " + prefab.name);

                // Marque comme placé si c’est un objet unique
                foreach (var key in uniquePlaced.Keys)
                {
                    if (prefab.name.Contains(key))
                    {
                        uniquePlaced[key] = true;
                        break;
                    }
                }
            }
        }
    }
}
