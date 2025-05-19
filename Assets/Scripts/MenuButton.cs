using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public GameObject menuPanel;
    public Button closeButton;
    public Button saveButton;
    public Button loadButton;
    public Button resetButton;
    public Button statsButton;

    void Start()
    {
        // Assignation des listeners pour chaque bouton
        GetComponent<Button>().onClick.AddListener(ToggleMenu);
        closeButton.onClick.AddListener(CloseMenu);
        saveButton.onClick.AddListener(SaveGame);
        loadButton.onClick.AddListener(LoadGame);
        resetButton.onClick.AddListener(ResetGame);
        statsButton.onClick.AddListener(OpenStats);

        // Masquer le menu au démarrage
        menuPanel.SetActive(false);
    }

    void ToggleMenu()
    {
        bool isActive = menuPanel.activeSelf;
        menuPanel.SetActive(!isActive);
        Debug.Log("Menu " + (isActive ? "fermé" : "ouvert"));
    }

    void CloseMenu()
    {
        menuPanel.SetActive(false);
        Debug.Log("Menu fermé via la croix.");
    }

    void SaveGame()
    {
        Debug.Log("💾 Sauvegarde de la partie...");
        SaveManager saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
            saveManager.SaveGame();
    }

    void LoadGame()
    {
        Debug.Log("📂 Chargement de la sauvegarde...");
        SaveManager saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
            saveManager.LoadGame();
    }

    void ResetGame()
    {
        Debug.Log("🔄 Réinitialisation de la partie...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void OpenStats()
    {
        Debug.Log("📊 Ouverture des statistiques...");
        // Implémente l'affichage des statistiques ici
    }
}
