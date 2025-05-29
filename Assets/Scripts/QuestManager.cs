using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class QuestManager : MonoBehaviour
{
    public TextMeshProUGUI questText;
    public Transform questListContainer;
    public static QuestManager Instance;
    public GameObject questItemPrefab;
    private int currentQuest = 0;
    private int researchPoints = 0;

    private List<string> questList = new List<string>
    {
        "Construisez un feu pour réchauffer les villageois",
        "Construisez une mairie pour gérer le village",
        "Collectez 100 unités de bois",
        "Débloquez une nouvelle compétence"
    };

    public delegate void QuestCompletedHandler(int questIndex);
    public event QuestCompletedHandler OnQuestCompleted;

    public delegate void QuestsLoadedHandler();
    public event QuestsLoadedHandler OnQuestsLoaded;

    void Awake()
    {
        Debug.Log("✅ QuestManager Awake appelé");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadGameProgress();
    }

    void Start()
    {
        ShowCurrentQuest();
        DisplayAllQuests();
    }

    public void CheckQuestProgress(string buildingTag)
    {
        if (buildingTag == "Feu" && currentQuest == 0)
        {
            CompleteQuest();
        }
        else if (buildingTag == "Mairie" && currentQuest == 1)
        {
            CompleteQuest();
        }
        // Ajoute d’autres conditions selon tes quêtes
    }


    void Update()
    { }


    void LoadGameProgress()
    {
        string path = Application.persistentDataPath + "/save.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bool feuPlace = data.buildings.Exists(b => b.tagName == "Feu");
            bool mairiePlace = data.buildings.Exists(b => b.tagName == "Mairie");

            if (feuPlace && currentQuest == 0)
            {
                CompleteQuest();  // Incrémente currentQuest à 1, +1 point, déclenche l’événement
            }

            if (mairiePlace && currentQuest == 1)
            {
                CompleteQuest();  // Incrémente currentQuest à 2, +1 point, déclenche l’événement
            }

            Debug.Log("✅ Chargement des quêtes depuis sauvegarde.");
            OnQuestsLoaded?.Invoke();
        }
    }


    void ShowCurrentQuest()
    {
        if (currentQuest < questList.Count)
            questText.text = "Quête : " + questList[currentQuest];
        else
            questText.text = "Quêtes terminées !";
    }

    void CompleteQuest()
    {
        Debug.Log($"✅ Quête {currentQuest} complétée !");
        researchPoints++;
        currentQuest++;
        OnQuestCompleted?.Invoke(currentQuest - 1);
        ShowCurrentQuest();
        DisplayAllQuests();
    }

    public string GetCurrentQuest()
    {
        if (currentQuest < questList.Count)
            return questList[currentQuest];
        else
            return null;
    }

    public int GetResearchPoints()
    {
        return researchPoints;
    }

    public void SpendResearchPoints(int amount)
    {
        researchPoints = Mathf.Max(0, researchPoints - amount);
    }

    public void DisplayAllQuests()
    {
        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < questList.Count; i++)
        {
            GameObject item = Instantiate(questItemPrefab, questListContainer);
            TextMeshProUGUI text = item.GetComponent<TextMeshProUGUI>();

            if (i < currentQuest)
                text.text = $"<s>{questList[i]}</s>";
            else if (i == currentQuest)
                text.text = $"<b>{questList[i]}</b>";
            else
                text.text = questList[i];
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public List<BuildingData> buildings = new List<BuildingData>();
    }

    [System.Serializable]
    public class BuildingData
    {
        public string prefabName;
        public Vector3 position;
        public string tagName;
    }
}
