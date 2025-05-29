using UnityEngine;
using TMPro;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;

    // Totaux cumulés
    public int totalWood = 0;
    public int totalStone = 0;
    public int totalIron = 0;
    public int totalGold = 0;
    public int totalFood = 0;
    public int totalPopulation = 3;

    // UI
    public TextMeshProUGUI woodStatText;
    public TextMeshProUGUI stoneStatText;
    public TextMeshProUGUI ironStatText;
    public TextMeshProUGUI goldStatText;
    public TextMeshProUGUI foodStatText;
    public TextMeshProUGUI populationStatText;
    public GameObject statsPanel;


    void Start()
    {
        UpdateStatsUI();
    }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddWood(int amount) { totalWood += amount; UpdateStatsUI(); }
    public void AddStone(int amount) { totalStone += amount; UpdateStatsUI(); }
    public void AddIron(int amount) { totalIron += amount; UpdateStatsUI(); }
    public void AddGold(int amount) { totalGold += amount; UpdateStatsUI(); }
    public void AddFood(int amount) { totalFood += amount; UpdateStatsUI(); }
    public void AddPopulation(int amount) { totalPopulation += amount; UpdateStatsUI(); }

    public void UpdateStatsUI()
    {
        if (woodStatText != null) woodStatText.text = totalWood.ToString();
        if (stoneStatText != null) stoneStatText.text = totalStone.ToString();
        if (ironStatText != null) ironStatText.text = totalIron.ToString();
        if (goldStatText != null) goldStatText.text = totalGold.ToString();
        if (foodStatText != null) foodStatText.text = totalFood.ToString();
        if (populationStatText != null) populationStatText.text = totalPopulation.ToString();
    }

    public void ToggleStatsPanel()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(!statsPanel.activeSelf);
        }
    }
}
