using UnityEngine;
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
        SaveData data = new SaveData();

        // ✅ Inclure tous les tags pertinents
        string[] tagsToSave = { "Building", "Feu", "Mairie" };

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
                    tagName = tag // ✅ On sauvegarde le tag
                };

                data.buildings.Add(bData);
            }
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
        string[] tagsToClear = { "Building", "Feu", "Mairie" };
        foreach (string tag in tagsToClear)
        {
            foreach (var building in GameObject.FindGameObjectsWithTag(tag))
            {
                Destroy(building);
            }
        }

        // Recréer les bâtiments sauvegardés
        foreach (var bData in data.buildings)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + bData.prefabName);

            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, bData.position, Quaternion.identity);

                newObj.tag = bData.tagName;
                newObj.layer = LayerMask.NameToLayer("Building");

                if (newObj.GetComponent<Collider2D>() == null)
                {
                    newObj.AddComponent<BoxCollider2D>();
                    Debug.Log($"🧩 Collider ajouté à {newObj.name}");
                }

                ColliderSettings settings = newObj.GetComponent<ColliderSettings>();
                if (settings != null)
                {
                    var collider = newObj.GetComponent<BoxCollider2D>();
                    collider.size = settings.customSize;
                    collider.offset = settings.customOffset;
                    Debug.Log($"📏 Collider ajusté via ColliderSettings pour {newObj.name}");
                }

                BuildingIdentifier identifier = newObj.AddComponent<BuildingIdentifier>();
                identifier.prefabName = bData.prefabName;

                // ✅ Juste ici : On recrée les PNJ pour les maisons
                if (bData.prefabName.Contains("maison"))
                {
                    ResourceManager.Instance.AddPopulation(3);
                    BuildManager.Instance.SpawnPNJsAround(bData.position, 3); // ✅ C'est ici !
                }


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
        // Raccourcis pour tests
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
