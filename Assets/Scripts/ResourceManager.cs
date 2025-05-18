using UnityEngine;
using TMPro;
using System.Collections;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    // UI Elements
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI ironText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI populationText; // ✅ Nouveau champ pour la population

    // Configuration clignotement
    public Color warningColor = Color.red;
    public float flashDuration = 0.2f;

    // Ressources
    public int wood = 0;
    public int food = 0;
    public int stone = 0;
    public int iron = 0;
    public int gold = 0;

    // ✅ Population
    public int population = 3; // Valeur de départ


    // Getters pour la sauvegarde
    public int GetWood() => wood;
    public int GetFood() => food;
    public int GetStone() => stone;
    public int GetIron() => iron;
    public int GetGold() => gold;

    // Setter pour recharger les ressources
    public void SetResources(int wood, int food, int stone, int iron, int gold)
    {
        this.wood = wood;
        this.food = food;
        this.stone = stone;
        this.iron = iron;
        this.gold = gold;
        UpdateUI();
    }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        wood = 10000;
        food = 10000;
        stone = 10000;
        iron = 10000;
        gold = 10000;

        UpdateUI();
    }

    void UpdateUI()
    {
        woodText.text = wood.ToString();
        foodText.text = food.ToString();
        stoneText.text = stone.ToString();
        ironText.text = iron.ToString();
        goldText.text = gold.ToString();

        if (populationText != null)
            populationText.text = population.ToString(); // ✅ Mise à jour de l'affichage population
    }

    // ✅ Gestion de la Population
    public void AddPopulation(int amount) 
    { 
        population += amount; 
        UpdateUI(); 
    }

    public void RemovePopulation(int amount) 
    { 
        population = Mathf.Max(0, population - amount); 
        UpdateUI(); 
    }

    public void SetPopulation(int value)
    {
        population = Mathf.Max(0, value);
        UpdateUI();
    }

    // Gestion des ressources
    public void AddWood(int amount) { wood += amount; UpdateUI(); }
    public void RemoveWood(int amount) { wood -= amount; UpdateUI(); }
    public void AddFood(int amount) { food += amount; UpdateUI(); }
    public void RemoveFood(int amount) { food -= amount; UpdateUI(); }
    public void AddStone(int amount) { stone += amount; UpdateUI(); }
    public void RemoveStone(int amount) { stone -= amount; UpdateUI(); }
    public void AddIron(int amount) { iron += amount; UpdateUI(); }
    public void RemoveIron(int amount) { iron -= amount; UpdateUI(); }
    public void AddGold(int amount) { gold += amount; UpdateUI(); }
    public void RemoveGold(int amount) { gold -= amount; UpdateUI(); }

    public bool HasEnoughResources(BuildingCost cost)
    {
        bool enough = wood >= cost.woodCost &&
                      food >= cost.foodCost &&
                      stone >= cost.stoneCost &&
                      iron >= cost.ironCost &&
                      gold >= cost.goldCost;

        if (!enough) FlashMissingResources(cost);

        return enough;
    }

    private void FlashMissingResources(BuildingCost cost)
    {
        if (cost.woodCost > wood) StartCoroutine(FlashCoroutine(woodText));
        if (cost.stoneCost > stone) StartCoroutine(FlashCoroutine(stoneText));
        if (cost.ironCost > iron) StartCoroutine(FlashCoroutine(ironText));
        if (cost.goldCost > gold) StartCoroutine(FlashCoroutine(goldText));
        if (cost.foodCost > food) StartCoroutine(FlashCoroutine(foodText));
    }

    private IEnumerator FlashCoroutine(TextMeshProUGUI textElement)
    {
        if (textElement == null) yield break;

        Color originalColor = textElement.color;
        for (int i = 0; i < 3; i++)
        {
            textElement.color = warningColor;
            yield return new WaitForSeconds(flashDuration);
            textElement.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        textElement.color = Color.black;
    }

    public void SpendResources(BuildingCost cost)
    {
        wood -= cost.woodCost;
        food -= cost.foodCost;
        stone -= cost.stoneCost;
        iron -= cost.ironCost;
        gold -= cost.goldCost;
        UpdateUI();
    }
}
