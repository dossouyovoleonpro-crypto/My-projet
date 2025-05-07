using UnityEngine;
using TMPro; // Assure-toi que tu utilises TextMesh Pro

public class ResourceManager : MonoBehaviour
{
    // Création de l'instance statique (Singleton)
    public static ResourceManager Instance;

    // Références TextMeshPro pour afficher les ressources dans l'UI
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI ironText;
    public TextMeshProUGUI goldText;

    // Ressources
    private int wood = 0;
    private int food = 0;
    private int stone = 0;
    private int iron = 0;
    private int gold = 0;

    // Méthode Awake pour assurer qu'il n'y ait qu'une seule instance du ResourceManager
    void Awake()
    {
        // Vérifie si l'instance est déjà assignée, sinon l'assigne à cette instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Si une instance existe déjà, détruit l'objet
        }
        else
        {
            Instance = this; // Assigne cette instance
            DontDestroyOnLoad(gameObject); // Ne détruire pas l'instance lors du chargement d'une nouvelle scène
        }
    }

void Start()
{
    // Initialisation des ressources de départ
    wood = 10000;  
    food = 10000;
    stone = 10000;  // Valeur initiale pour la pierre
    iron = 10000;   // Valeur initiale pour le fer
    gold = 10000;    // Valeur initiale pour l'or
    
    // Mise à jour de l'UI
    UpdateUI();
}


    // Met à jour l'affichage des ressources
    void UpdateUI()
    {
        woodText.text = wood.ToString();    // Affiche le bois
        foodText.text = food.ToString();    // Affiche la nourriture
        stoneText.text = stone.ToString();  // Affiche la pierre
        ironText.text = iron.ToString();    // Affiche le fer
        goldText.text = gold.ToString();    // Affiche l'or
    }

    // Ajouter du bois
    public void AddWood(int amount)
    {
        wood += amount;
        UpdateUI();
    }

    // Retirer du bois
    public void RemoveWood(int amount)
    {
        wood -= amount;
        UpdateUI();
    }

    // Ajouter de la nourriture
    public void AddFood(int amount)
    {
        food += amount;
        UpdateUI();
    }

    // Retirer de la nourriture
    public void RemoveFood(int amount)
    {
        food -= amount;
        UpdateUI();
    }

    // Ajouter de la pierre
    public void AddStone(int amount)
    {
        stone += amount;
        UpdateUI();
    }

    // Retirer de la pierre
    public void RemoveStone(int amount)
    {
        stone -= amount;
        UpdateUI();
    }

    // Ajouter du fer
    public void AddIron(int amount)
    {
        iron += amount;
        UpdateUI();
    }

    // Retirer du fer
    public void RemoveIron(int amount)
    {
        iron -= amount;
        UpdateUI();
    }

    // Ajouter de l'or
    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    // Retirer de l'or
    public void RemoveGold(int amount)
    {
        gold -= amount;
        UpdateUI();
    }

    // Vérifie si les ressources sont suffisantes avant la construction
    public bool HasEnoughResources(BuildingCost cost)
    {
        return wood >= cost.woodCost &&
               food >= cost.foodCost &&
               stone >= cost.stoneCost &&
               iron >= cost.ironCost &&
               gold >= cost.goldCost;
    }

    // Déduit les ressources après la construction
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
