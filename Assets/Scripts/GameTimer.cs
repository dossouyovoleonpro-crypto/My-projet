using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;  // Assigne dans l'inspecteur le texte à afficher
    public float acceleratedSpeed = 5f; // Vitesse du temps accéléré

    private float elapsedTime = 0f;
    private bool isPaused = false;
    private bool isAccelerated = false;

    private SaveRessource saveRessource; // Référence vers le script de sauvegarde

    void Start()
    {
        // Trouve le script SaveRessource dans la scène
        saveRessource = FindFirstObjectByType<SaveRessource>();
        if (saveRessource != null)
        {
            elapsedTime = saveRessource.GetElapsedTime();
            Debug.Log($"🕒 Temps initial chargé : {elapsedTime} secondes");
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun SaveRessource trouvé, temps démarré à zéro.");
        }
    }

    void Update()
    {
        // Gestion des touches
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            Debug.Log(isPaused ? "⏸️ Jeu en pause" : "▶️ Jeu repris");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isAccelerated = !isAccelerated;
            Debug.Log(isAccelerated ? "⏩ Accélération activée" : "⏱️ Retour à la vitesse normale");
        }

        if (!isPaused)
        {
            float delta = Time.deltaTime * (isAccelerated ? acceleratedSpeed : 1f);
            elapsedTime += delta;

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
