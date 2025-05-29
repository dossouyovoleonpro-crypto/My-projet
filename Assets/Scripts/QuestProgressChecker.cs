using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class QuestProgressChecker : MonoBehaviour
{
    public static QuestProgressChecker Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Vérifie dès l'Awake si des bâtiments sont déjà placés dans la sauvegarde
        CheckProgressFromSave();
    }

    public void CheckQuestProgress(GameObject placedBuilding)
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogError("❌ QuestManager.Instance est null !");
            return;
        }

        if (placedBuilding.CompareTag("Feu"))
        {
            QuestManager.Instance.CheckQuestProgress("Feu");
        }
        else if (placedBuilding.CompareTag("Mairie"))
        {
            QuestManager.Instance.CheckQuestProgress("Mairie");
        }
    }


    private void CheckProgressFromSave()
    {
        string path = Application.persistentDataPath + "/save.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bool feuPlace = data.buildings.Exists(b => b.tagName == "Feu");
            bool mairiePlace = data.buildings.Exists(b => b.tagName == "Mairie");

            if (feuPlace)
            {
                Debug.Log("✅ Feu déjà présent dans la sauvegarde, mise à jour de la quête");
                QuestManager.Instance.CheckQuestProgress("Feu");
            }

            if (mairiePlace)
            {
                Debug.Log("✅ Mairie déjà présente dans la sauvegarde, mise à jour de la quête");
                QuestManager.Instance.CheckQuestProgress("Mairie");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun fichier de sauvegarde trouvé.");
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public List<QuestManager.BuildingData> buildings = new List<QuestManager.BuildingData>();
    }
}
