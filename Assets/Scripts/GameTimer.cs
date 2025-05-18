using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;  // Assigne dans l'inspecteur le texte à afficher
    public float acceleratedSpeed = 5f; // Vitesse du temps accéléré

    private float elapsedTime = 0f;
    private bool isPaused = false;
    private bool isAccelerated = false;

    void Update()
    {
        // Gestion des touches
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            Debug.Log(isPaused ? "⏸️ Jeu en pause" : "▶️ Jeu repris");
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Nouveau pour clavier AZERTY
        {
            isAccelerated = !isAccelerated;
            Debug.Log(isAccelerated ? "⏩ Accélération activée" : "⏱️ Retour à la vitesse normale");
        }

        // Mise à jour du temps si non en pause
        if (!isPaused)
        {
            float delta = Time.deltaTime * (isAccelerated ? acceleratedSpeed : 1f);
            elapsedTime += delta;

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}