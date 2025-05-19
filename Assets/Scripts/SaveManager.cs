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

        string[] tagsToClear = { "Building", "Feu", "Mairie" };
        foreach (string tag in tagsToClear)
        {
            foreach (var building in GameObject.FindGameObjectsWithTag(tag))
            {
                Destroy(building);
            }
        }

        foreach (var bData in currentSaveData.buildings)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + bData.prefabName);
            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, bData.position, Quaternion.identity);
                newObj.tag = bData.tagName;

                var identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = bData.prefabName;
            }
        }

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) SaveGame();
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();
    }
}
