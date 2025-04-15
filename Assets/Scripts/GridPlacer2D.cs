using UnityEngine;

public class GridPlacer2D : MonoBehaviour
{
    public LayerMask clickableLayer;  // Ce layer ne doit inclure que les objets cliquables (par ex. chemins)

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Si on clique avec le bouton gauche
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // Position de la souris dans le monde

            // Raycast pour détecter l'objet sous le clic, avec un LayerMask spécifique
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, clickableLayer);

            if (hit.collider == null) return;  // Si rien n'est touché, on arrête ici

            if (BuildManager.Instance.IsDeleteMode())  // Si le mode suppression est activé
            {
                // 🗑️ On supprime l'objet cliqué (seulement si c'est dans le bon layer)
                Destroy(hit.collider.gameObject);
                Debug.Log("Objet supprimé : " + hit.collider.name);
            }
            else
            {
                // Si on est en mode placement, on place le prefab sélectionné
                GameObject prefab = BuildManager.Instance.GetSelectedPrefab();
                if (prefab == null) return;  // Si aucun prefab n'est sélectionné, on arrête ici

                // Position de spawn du prefab (pour bien l'aligner sur la grille)
                Vector2 spawnPos = new Vector2(
                    Mathf.Floor(mousePos.x) + 0.5f,
                    Mathf.Floor(mousePos.y) + 0.5f
                );

                Instantiate(prefab, spawnPos, Quaternion.identity);  // On place le prefab à la position calculée
                Debug.Log("Prefab placé : " + prefab.name);
            }
        }
    }
}
