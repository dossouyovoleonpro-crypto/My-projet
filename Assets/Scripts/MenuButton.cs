using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class MenuButton : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject questPanel;
    public GameObject skillTreePanel;


    public List<Button> closeButtons;
    public Button questButton;
    public Button skillTreeButton;
    public Button resetButton;
    public Button statsButton;



    void Start()
    {
        foreach (Button btn in closeButtons)
        {
            btn.onClick.AddListener(CloseMenu);
        }

        GetComponent<Button>().onClick.AddListener(ToggleMenu);
        resetButton.onClick.AddListener(ResetGame);
        statsButton.onClick.AddListener(OpenStats);
        questButton.onClick.AddListener(OpenQuestPanel);
        skillTreeButton.onClick.AddListener(OpenSkillTreePanel);

        questPanel.SetActive(false);
        skillTreePanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    void ToggleMenu()
    {
        bool isActive = menuPanel.activeSelf;
        menuPanel.SetActive(!isActive);
    }

    void CloseMenu()
    {
        menuPanel.SetActive(false);
    }

    void OpenSkillTreePanel()
    {
        skillTreePanel.SetActive(true);
        questPanel.SetActive(false);
    }

    void ResetGame()
    {
        Debug.Log("🔄 Réinitialisation de la map à l'état initial...");

        SaveManager saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
        {
            saveManager.ResetToBaseMap();
        }
        if (GameEvents.OnResetGame != null)
        {
            GameEvents.OnResetGame.Invoke();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

        void OpenQuestPanel()
    {
        menuPanel.SetActive(false);  // Ferme le menu principal
        questPanel.SetActive(true);  // Ouvre le panneau Quête
        Debug.Log("📜 Panneau Quêtes ouvert.");
    }


    void OpenStats()
    {
        Debug.Log("📊 Ouverture des statistiques...");
        // Implémente l'affichage des statistiques ici
    }
}
