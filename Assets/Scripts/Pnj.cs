using UnityEngine;

public class Pnj : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Vector3[] path;  // Chemin à suivre
    private int currentPoint = 0;

    // Définir la hauteur du terrain (si nécessaire)
    private float terrainY = -2.7f;  // La position Y du terrain

    void Start()
    {
        // Initialisation du chemin avec des points de test
        path = new Vector3[] {
            new Vector3(0, 0, 0),   // Point de départ
            new Vector3(5, 0, 0),   // Premier point à atteindre
            new Vector3(5, 0, 5),   // Deuxième point
            new Vector3(0, 0, 5)    // Troisième point
        };

        // Positionner le PNJ juste au-dessus du terrain (Y > -2.7)
        transform.position = new Vector3(0, 1, 0);  // Assurez-vous que la position Y est au-dessus du terrain
    }

    void Update()
    {
        // Appeler la fonction de déplacement chaque frame
        Move();
    }

    void Move()
    {
        if (path.Length == 0)
            return;

        // Déplacer le PNJ vers le prochain point
        transform.position = Vector3.MoveTowards(transform.position, path[currentPoint], moveSpeed * Time.deltaTime);

        // Vérifier si le PNJ est sous le terrain (Y < terrainY)
        if (transform.position.y < terrainY)
        {
            // Remettre le PNJ juste au-dessus du terrain
            transform.position = new Vector3(transform.position.x, terrainY + 1f, transform.position.z);  // Position Y au-dessus du terrain
        }

        // Si le PNJ atteint le point cible, on passe au point suivant
        if (transform.position == path[currentPoint])
        {
            currentPoint++;
            if (currentPoint >= path.Length)
                currentPoint = 0;  // Retour au début du chemin
        }
    }
}
