using UnityEngine;
using System.IO;

[System.Serializable]
public class RessourceData
{
    public int wood;
    public int food;
    public int stone;
    public int iron;
    public int gold;
    public int population;
    public float elapsedTime;
}

public class SaveRessource : MonoBehaviour
{
    private string savePath;
    private RessourceData data;
    private float elapsedTime;

    void Awake()
    {
        savePath = Application.persistentDataPath + "/ressources.json";
        data = new RessourceData();
        LoadRessources();
    }

    void Start()
    {
        LoadRessources();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public void SaveRessourcesData()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("❌ ResourceManager.Instance est null. Impossible de sauvegarder.");
            return;
        }

        data.wood = ResourceManager.Instance.GetWood();
        data.food = ResourceManager.Instance.GetFood();
        data.stone = ResourceManager.Instance.GetStone();
        data.iron = ResourceManager.Instance.GetIron();
        data.gold = ResourceManager.Instance.GetGold();
        data.population = ResourceManager.Instance.population;
        data.elapsedTime = elapsedTime;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"💾 Sauvegarde terminée ! Données : " +
                  $"Wood: {data.wood}, Food: {data.food}, Stone: {data.stone}, Iron: {data.iron}, Gold: {data.gold}, " +
                  $"Population: {data.population}, Time: {data.elapsedTime:F2}");
    }

    public void ResetElapsedTime()
    {
        elapsedTime = 0f;
        Debug.Log("⏳ Temps de jeu réinitialisé à zéro.");
    }

    public void LoadRessources()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠️ Aucune sauvegarde trouvée. Création d'une nouvelle sauvegarde.");
            SaveRessourcesData();
            return;
        }

        string json = File.ReadAllText(savePath);
        data = JsonUtility.FromJson<RessourceData>(json);

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("❌ ResourceManager.Instance est null. Impossible de charger.");
            return;
        }

        ResourceManager.Instance.SetResources(data.wood, data.food, data.stone, data.iron, data.gold);
        ResourceManager.Instance.SetPopulation(data.population);
        elapsedTime = data.elapsedTime;

        Debug.Log($"📥 Chargement terminé ! Données : " +
                  $"Wood: {data.wood}, Food: {data.food}, Stone: {data.stone}, Iron: {data.iron}, Gold: {data.gold}, " +
                  $"Population: {data.population}, Time: {data.elapsedTime:F2}");
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void ResetAllResources()
    {
        data.wood = 0;
        data.food = 0;
        data.stone = 0;
        data.iron = 0;
        data.gold = 0;
        data.population = 3;
        data.elapsedTime = 0f;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("💾 Ressources et temps réinitialisés dans le fichier ressources.json");
    }
}
