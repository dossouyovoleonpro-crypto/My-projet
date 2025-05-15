using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class BuildingData
{
    public string prefabName;
    public Vector3 position;
}

[System.Serializable]
public class SaveData
{
    public List<BuildingData> buildings = new List<BuildingData>();
}

public class SaveManager : MonoBehaviour
{
    private string savePath;

    void Awake()
    {
        savePath = Application.persistentDataPath + "/save.json";
        Debug.Log("💾 [SaveManager] AWAKENED !");
        Debug.Log($"💾 Chemin de sauvegarde : {savePath}");
    }

    void Start()
    {
        Debug.Log("📂 [SaveManager] Start() appelé, tentative de chargement de la sauvegarde.");
        LoadGame();
    }

    void OnApplicationQuit()
    {
        Debug.Log("⛔ [SaveManager] OnApplicationQuit() appelé, sauvegarde automatique en cours...");
        SaveGame();
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        foreach (var building in GameObject.FindGameObjectsWithTag("Building"))
        {
            BuildingIdentifier identifier = building.GetComponent<BuildingIdentifier>();
            string prefabName = identifier != null ? identifier.prefabName : building.name.Replace("(Clone)", "");

            BuildingData bData = new BuildingData
            {
                prefabName = prefabName,
                position = building.transform.position
            };

            data.buildings.Add(bData);
        }

        string json = JsonUtility.ToJson(data, true);
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
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Supprimer les bâtiments existants
        foreach (var building in GameObject.FindGameObjectsWithTag("Building"))
        {
            Destroy(building);
        }

        // Recréer les bâtiments sauvegardés
        foreach (var bData in data.buildings)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + bData.prefabName);

            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, bData.position, Quaternion.identity);

                // Réassigner l’identifiant pour la prochaine sauvegarde
                BuildingIdentifier identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = bData.prefabName;
            }
            else
            {
                Debug.LogWarning($"❌ [SaveManager] Prefab non trouvé : {bData.prefabName}. Vérifie qu'il est bien dans 'Resources/Prefabs/'.");
            }
        }

        Debug.Log("📥 [SaveManager] Chargement terminé !");
    }

    void Update()
    {
        // Raccourcis pour tests rapides
        if (Input.GetKeyDown(KeyCode.K)) // K = Save
        {
            Debug.Log("💾 [SaveManager] Sauvegarde manuelle via touche K.");
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L)) // L = Load
        {
            Debug.Log("📂 [SaveManager] Chargement manuel via touche L.");
            LoadGame();
        }
    }
}