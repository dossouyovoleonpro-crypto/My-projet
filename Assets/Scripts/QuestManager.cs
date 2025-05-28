using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public TextMeshProUGUI questText;  // Texte affichant la quête active
    public Transform questListContainer;  // Le Content du Scroll View
    public GameObject questItemPrefab;    // Prefab d’un texte pour chaque quête
    private int currentQuest = 0;
    private int researchPoints = 0;

    private List<string> questList = new List<string>
    {
        "Construisez un feu pour réchauffer les villageois",
        "Construisez une mairie pour gérer le village",
        "Collectez 100 unités de bois",
        "Débloquez une nouvelle compétence"
    };

    void Start()
    {
        ShowCurrentQuest();
        DisplayAllQuests();
    }

    void Update()
    {
        if (currentQuest == 0 && GameObject.FindGameObjectWithTag("Feu") != null)
            CompleteQuest();
        else if (currentQuest == 1 && GameObject.FindGameObjectWithTag("Mairie") != null)
            CompleteQuest();
        // Ajoute d'autres conditions pour d’autres quêtes ici si nécessaire
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
        Debug.Log($"✅ Quête {currentQuest} complétée !");
        researchPoints++;
        currentQuest++;
        ShowCurrentQuest();
        DisplayAllQuests();
    }

    public int GetResearchPoints() => researchPoints;

    public void DisplayAllQuests()
    {
        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);  // Nettoie l’ancienne liste
        }

        for (int i = 0; i < questList.Count; i++)
        {
            GameObject item = Instantiate(questItemPrefab, questListContainer);
            TextMeshProUGUI text = item.GetComponent<TextMeshProUGUI>();

            if (i < currentQuest)
                text.text = $"<s>{questList[i]}</s>";  // Rayé si complété
            else if (i == currentQuest)
                text.text = $"<b>{questList[i]}</b>";  // En gras si en cours
            else
                text.text = questList[i];  // Normal si pas encore atteinte
        }
    }
}
