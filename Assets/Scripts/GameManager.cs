using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Assurez-vous que les préfabriqués et les Tilemaps sont assignés dans l'inspecteur Unity.
    public GameObject PnjPrefab;    // Préfabriqué de PNJ
    public Tilemap TerrainMap;      // Tilemap pour le terrain
    public Tilemap ObstacleMap;     // Tilemap pour les obstacles
    public Vector2Int GardenBottomLeft; // Coin en bas à gauche de la zone de jardin
    public Vector2Int GardenTopRight;  // Coin en haut à droite de la zone de jardin
    public int pnjCount = 5;        // Nombre de PNJ à générer

    void Start()
    {
        // Assure-toi que les Tilemaps sont assignées avant de commencer.
        if (TerrainMap == null || ObstacleMap == null)
        {
            Debug.LogError("Les Tilemaps ne sont pas assignées dans l'inspecteur !");
            return;
        }


        // Appelle la fonction pour générer les PNJ.
        SpawnPNJs();
    }

    void SpawnPNJs()
    {
        List<Vector3> validPositions = new List<Vector3>();

        // On parcourt la zone définie par GardenBottomLeft et GardenTopRight.
        for (int x = GardenBottomLeft.x; x <= GardenTopRight.x; x++)
        {
            for (int y = GardenBottomLeft.y; y <= GardenTopRight.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                // Vérifie qu'il n'y a pas d'obstacle et que c'est un endroit valide sur le terrain.
                if (!ObstacleMap.HasTile(cell) && TerrainMap.HasTile(cell))
                {
                    Vector3 worldPos = TerrainMap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0); // Calculer la position dans le monde.
                    validPositions.Add(worldPos);
                }
            }
        }

        // Si nous avons des positions valides, générer les PNJ à des positions aléatoires.
        for (int i = 0; i < pnjCount && validPositions.Count > 0; i++)
        {
            int randIndex = Random.Range(0, validPositions.Count);
            Vector3 spawnPos = validPositions[randIndex];
            Instantiate(PnjPrefab, spawnPos, Quaternion.identity); // Créer un PNJ à la position sélectionnée.
            validPositions.RemoveAt(randIndex); // Éviter de doubler les positions.
        }
    }
}
