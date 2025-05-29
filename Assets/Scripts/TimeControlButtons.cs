using UnityEngine;

public class TimeControlButtons : MonoBehaviour
{
    public void PauseTime()
    {
        TimeManager.Instance.SetTimeScale(0f);
    }

    public void NormalTime()
    {
        TimeManager.Instance.SetTimeScale(1f);
    }

    public void FastTime()
    {
        TimeManager.Instance.SetTimeScale(2f); // Ou 2x selon ton besoin
    }
}
