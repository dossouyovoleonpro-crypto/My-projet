using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float timeScale = 1f;

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

    public void SetTimeScale(float scale)
    {
        timeScale = scale;
        Time.timeScale = scale;  // Affecte la vitesse globale du jeu
        Debug.Log($"⏱️ Temps réglé sur {scale}x");
    }

    public float GetAdjustedSpeed(float baseSpeed)
    {
        return baseSpeed * timeScale;  // Ajuste la vitesse individuelle
    }
}
