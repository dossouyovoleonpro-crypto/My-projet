using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class BuildingData
{
    public string prefabName;
    public Vector3 position;
    public string tagName;
}

[System.Serializable]
public class TileDeletionData
{
    public List<Vector3IntSerializable> deletedTiles = new List<Vector3IntSerializable>();
}

[System.Serializable]
public struct Vector3IntSerializable
{
    public int x, y, z;

    public Vector3IntSerializable(Vector3Int vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }

    public Vector3Int ToVector3Int() => new Vector3Int(x, y, z);
}

[System.Serializable]
public class SaveData
{
    public List<BuildingData> buildings = new List<BuildingData>();
    public TileDeletionData tileDeletions = new TileDeletionData();
}

[System.Serializable]
public class TileData
{
    public string tileName;
    public Vector3Int position;
}

[System.Serializable]
public class TilemapSaveData
{
    public List<TileData> tiles = new List<TileData>();
}

public class SaveManager : MonoBehaviour
{
    private string savePath;
    private SaveData currentSaveData = new SaveData();

    public Tilemap obstacleMap; // Assigner dans l'inspecteur à 'TerrainObstacle'

    void Awake()
    {
        savePath = Application.persistentDataPath + "/save.json";
        Debug.Log($"💾 [SaveManager] Chemin de sauvegarde : {savePath}");
    }

    void Start()
    {
        LoadGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        string[] tagsToSave = { "Building", "Feu", "Mairie" };
        currentSaveData.buildings.Clear();

        foreach (string tag in tagsToSave)
        {
            foreach (var building in GameObject.FindGameObjectsWithTag(tag))
            {
                BuildingIdentifier identifier = building.GetComponent<BuildingIdentifier>();
                string prefabName = identifier != null ? identifier.prefabName : building.name.Replace("(Clone)", "");

                BuildingData bData = new BuildingData
                {
                    prefabName = prefabName,
                    position = building.transform.position,
                    tagName = tag
                };

                currentSaveData.buildings.Add(bData);
            }
        }

        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"✅ [SaveManager] Sauvegarde effectuée à : {savePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠️ [SaveManager] Aucune sauvegarde trouvée !");
            return;
        }

        string json = File.ReadAllText(savePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(json);

        // Supprimer tous les bâtiments existants
        string[] tagsToClear = { "Building", "Feu", "Mairie" };
        foreach (string tag in tagsToClear)
        {
            foreach (var building in GameObject.FindGameObjectsWithTag(tag))
            {
                Destroy(building);
            }
        }

        // Recréer les bâtiments
        foreach (var bData in currentSaveData.buildings)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + bData.prefabName);
            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, bData.position, Quaternion.identity);
                newObj.tag = bData.tagName;
                newObj.layer = LayerMask.NameToLayer("Building");

                var identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = bData.prefabName;
                if (bData.prefabName.ToLower().Contains("feu"))
                    newObj.tag = "Feu";
                else if (bData.prefabName.ToLower().Contains("mairie"))
                    newObj.tag = "Mairie";
                else
                    newObj.tag = "Building";

                // ✅ Si maison ou foyer, on recrée les villageois
                if (bData.prefabName.ToLower().Contains("maison"))
                {
                    ResourceManager.Instance.AddPopulation(0);
                    BuildManager.Instance.SpawnPNJsAround(bData.position, 1, newObj.transform);
                }
                else if (bData.prefabName.ToLower().Contains("foyer"))
                {
                    ResourceManager.Instance.AddPopulation(0);
                    BuildManager.Instance.SpawnPNJsAround(bData.position, 2, newObj.transform);
                }

                if (newObj.GetComponent<Collider2D>() == null)
                {
                    newObj.AddComponent<BoxCollider2D>();
                    Debug.Log($"🧩 Collider ajouté à {newObj.name}");
                }
            }
        }

        // Supprimer les Tiles récoltées
        foreach (var serializedPos in currentSaveData.tileDeletions.deletedTiles)
        {
            Vector3Int pos = serializedPos.ToVector3Int();
            if (obstacleMap != null)
            {
                obstacleMap.SetTile(pos, null);
                Debug.Log($"🗑️ [SaveManager] Suppression de la Tile Ressource à : {pos}");
            }
        }

        Debug.Log("📥 [SaveManager] Chargement terminé !");
    }


    public void RegisterTileDeletion(Vector3Int tilePosition)
    {
        var serializablePos = new Vector3IntSerializable(tilePosition);

        if (!currentSaveData.tileDeletions.deletedTiles.Contains(serializablePos))
        {
            currentSaveData.tileDeletions.deletedTiles.Add(serializablePos);
            Debug.Log($"🗑️ [SaveManager] Tile supprimée enregistrée à : {tilePosition}");
            SaveGame(); // Sauvegarde immédiate après suppression
        }
    }

    public bool IsTileDeleted(Vector3Int pos)
    {
        return currentSaveData.tileDeletions.deletedTiles.Contains(new Vector3IntSerializable(pos));
    }

    public void LoadTilemap(Tilemap tilemap, string saveName)
    {
        string path = Application.persistentDataPath + "/Saves/" + saveName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogWarning($"⚠️ [SaveManager] Fichier {saveName}.json introuvable.");
            return;
        }

        string json = File.ReadAllText(path);
        TilemapSaveData tileData = JsonUtility.FromJson<TilemapSaveData>(json);

        tilemap.ClearAllTiles();

        foreach (var tileInfo in tileData.tiles)
        {
            TileBase tile = Resources.Load<TileBase>($"Tiles/{tileInfo.tileName}");
            if (tile != null)
                tilemap.SetTile(tileInfo.position, tile);
        }

        Debug.Log($"📥 [SaveManager] Map {saveName} chargée.");
    }

    public void ResetToBaseMap()
    {
        string[] tagsToClear = { "Building", "Feu", "Mairie" };

        foreach (string tag in tagsToClear)
        {
            foreach (var building in GameObject.FindGameObjectsWithTag(tag))
            {
                bool hasObstacle = false;

                ColliderSettings settings = building.GetComponent<ColliderSettings>();
                Vector2 size = settings != null ? settings.customSize : new Vector2(1, 1);
                Vector2 offset = settings != null ? settings.customOffset : Vector2.zero;

                Vector3 basePos = building.transform.position;
                Vector3Int startCell = obstacleMap.WorldToCell(basePos + (Vector3)offset - new Vector3(size.x / 2f, size.y / 2f, 0));


                for (int x = 0; x < (int)size.x; x++)
                {
                    for (int y = 0; y < (int)size.y; y++)
                    {
                        Vector3Int cell = new Vector3Int(startCell.x + x, startCell.y + y, 0);
                        if (obstacleMap.HasTile(cell))
                        {
                            hasObstacle = true;
                            break;
                        }
                    }
                    if (hasObstacle) break;
                }

                if (hasObstacle)
                {
                    Destroy(building);
                    Debug.Log($"🧹 [SaveManager] {tag} détruit car superposé à un obstacle : {building.name}");
                }

                // Supprimer le ghost de placement s’il existe
                if (BuildManager.Instance != null)
                {
                    BuildManager.Instance.ClearGhost();
                }

            }
        }
    }
}
