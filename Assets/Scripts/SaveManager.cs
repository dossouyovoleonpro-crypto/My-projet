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

    public int totalWood;
    public int totalStone;
    public int totalIron;
    public int totalGold;
    public int totalFood;
    public int totalPopulation = 3;
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
    private bool isResetInProgress = false;

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


        currentSaveData.totalWood = StatManager.Instance.totalWood;
        currentSaveData.totalStone = StatManager.Instance.totalStone;
        currentSaveData.totalIron = StatManager.Instance.totalIron;
        currentSaveData.totalGold = StatManager.Instance.totalGold;
        currentSaveData.totalFood = StatManager.Instance.totalFood;
        currentSaveData.totalPopulation = StatManager.Instance.totalPopulation;

        //Debug.Log($"✅ [SaveManager] Sauvegarde effectuée à : {savePath}");
    }

    public void LoadGame(bool skipTileDeletions = false)
{
    if (!File.Exists(savePath))
    {
        Debug.LogWarning("⚠️ [SaveManager] Aucune sauvegarde trouvée !");
        return;
    }

    string json = File.ReadAllText(savePath);
    currentSaveData = JsonUtility.FromJson<SaveData>(json);
    
    StatManager.Instance.totalWood = currentSaveData.totalWood;
    StatManager.Instance.totalStone = currentSaveData.totalStone;
    StatManager.Instance.totalIron = currentSaveData.totalIron;
    StatManager.Instance.totalGold = currentSaveData.totalGold;
    StatManager.Instance.totalFood = currentSaveData.totalFood;
    StatManager.Instance.totalPopulation = currentSaveData.totalPopulation;

    StatManager.Instance.UpdateStatsUI();

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

                if (bData.prefabName.ToLower().Contains("maison"))
                {
                    BuildManager.Instance.SpawnPNJsAround(bData.position, 3, newObj.transform);
                }
                else if (bData.prefabName.ToLower().Contains("foyer"))
                {
                    BuildManager.Instance.SpawnPNJsAround(bData.position, 5, newObj.transform);
                }

                if (newObj.GetComponent<Collider2D>() == null)
                {
                    newObj.AddComponent<BoxCollider2D>();
                    Debug.Log($"🧩 Collider ajouté à {newObj.name}");
                }
            
                if (bData.prefabName.ToLower().Contains("entrepot"))
                {
                    ResourceManager.Instance?.AddCapacityBonus(100);
                    Debug.Log("📦 Entrepôt chargé : +100 capacité.");
                }
        }
    }

    // Supprimer les Tiles récoltées sauf si on est en reset
    if (!skipTileDeletions)
    {
        foreach (var serializedPos in currentSaveData.tileDeletions.deletedTiles)
        {
            Vector3Int pos = serializedPos.ToVector3Int();
            if (obstacleMap != null)
            {
                obstacleMap.SetTile(pos, null);
                //Debug.Log($"🗑️ [SaveManager] Suppression de la Tile Ressource à : {pos}");
            }
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
            //Debug.Log($"🗑️ [SaveManager] Tile supprimée enregistrée à : {tilePosition}");
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
    isResetInProgress = true;
    Debug.Log("🧹 [SaveManager] Réinitialisation de la Tilemap avec la BaseMap...");

    string[] tagsToClear = { "Building", "Feu", "Mairie" };
    foreach (string tag in tagsToClear)
    {
        foreach (var building in GameObject.FindGameObjectsWithTag(tag))
        {
            Destroy(building);
        }
    }

    foreach (var pnj in GameObject.FindGameObjectsWithTag("PNJ"))
    {
        Destroy(pnj);
    }

    // Recharge la carte depuis BaseMap
    LoadTilemap(obstacleMap, "BaseMap");

    // 🔸 Supprime toutes les infos des tiles supprimées et sauvegarde le fichier
    currentSaveData.tileDeletions.deletedTiles.Clear();
    File.WriteAllText(savePath, JsonUtility.ToJson(currentSaveData, true));
    Debug.Log("💾 Tiles supprimées effacées du fichier sauvegarde.");

    var saveRessource = FindFirstObjectByType<SaveRessource>();
    if (saveRessource != null)
    {
        saveRessource.ResetElapsedTime();
        saveRessource.ResetAllResources();
        Debug.Log("🔄 Reset des ressources");
    }

    string emptyJson = "{}";
    File.WriteAllText(savePath, emptyJson);
    Debug.Log("🧹 [SaveManager] Fichier save.json réinitialisé à vide.");


    Debug.Log("🛑 Le jeu va se fermer.");
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
}





    public void SaveRessourcesIfNotReset()
    {
        if (isResetInProgress)
        {
            Debug.Log("⛔ Pas de sauvegarde car reset en cours.");
            return;  // 🔴 Bloque la sauvegarde si reset
        }

        var saveRessource = FindFirstObjectByType<SaveRessource>();
        if (saveRessource != null)
        {
            saveRessource.SaveRessourcesData();
            Debug.Log("💾 Sauvegarde automatique des ressources.");
        }
    }

    // Exemple de déclenchement (à appeler quand on quitte le jeu)
    private void OnApplicationQuit()
    {
        SaveRessourcesIfNotReset();
    }

}