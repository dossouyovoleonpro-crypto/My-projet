using UnityEngine;
using UnityEngine.Rendering.Universal; // ✅ Important pour Light2D

public class DayNightCycle : MonoBehaviour
{
    public Light2D globalLight; // ✅ Assigne ici ta Global Light 2D
    public float dayDuration = 10f;
    public float nightDuration = 2f;

    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);

    private float timer = 0f;
    private bool isDay = true;

    void Start()
    {
        if (globalLight == null)
        {
            Debug.LogError("❌ [DayNightCycle] Aucune Global Light 2D assignée !");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isDay && timer >= dayDuration)
        {
            SwitchToNight();
        }
        else if (!isDay && timer >= nightDuration)
        {
            SwitchToDay();
        }
    }

    void SwitchToNight()
    {
        isDay = false;
        timer = 0f;
        if (globalLight != null)
        {
            globalLight.color = nightColor;
            globalLight.intensity = 0.6f; // Diminue la lumière
        }
        Debug.Log("🌙 Nuit !");
    }

    void SwitchToDay()
    {
        isDay = true;
        timer = 0f;
        if (globalLight != null)
        {
            globalLight.color = dayColor;
            globalLight.intensity = 1f; // Rétablit la lumière
        }
        Debug.Log("☀️ Jour !");
    }

    public bool IsDayTime()
    {
        return isDay;
    }

}
