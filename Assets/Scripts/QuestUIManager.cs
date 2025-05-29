using UnityEngine;
using TMPro;

public class QuestUIManager : MonoBehaviour
{
    public TextMeshProUGUI questUIText;
    public QuestManager questManager;

    void Start()
    {
        if (questManager == null)
            questManager = FindFirstObjectByType<QuestManager>();

        if (questManager != null)
        {
            questManager.OnQuestCompleted += HandleQuestCompleted;
            questManager.OnQuestsLoaded += HandleQuestsLoaded;
        }

        UpdateQuestUIText(questManager?.GetCurrentQuest());
    }


    void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.OnQuestCompleted -= HandleQuestCompleted;
            questManager.OnQuestsLoaded -= HandleQuestsLoaded;
        }
    }

    void HandleQuestCompleted(int questIndex)
    {
        UpdateQuestUIText(questManager.GetCurrentQuest());  // Réactif dès qu'une quête est validée
    }

    void HandleQuestsLoaded()
    {
        UpdateQuestUIText(questManager.GetCurrentQuest());
    }

    void UpdateQuestUIText(string currentQuest)
    {
        if (!string.IsNullOrEmpty(currentQuest))
            questUIText.text = "Quête : " + currentQuest;
        else
            questUIText.text = "Quêtes terminées !";
    }
}
