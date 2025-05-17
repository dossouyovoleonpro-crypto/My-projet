using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    private bool isDeleteModeActive = false;
    private Button button;
    public Color activeColor = Color.red;
    public Color normalColor = Color.white;

    void Start()
    {
        button = GetComponent<Button>();
        GetComponent<Button>().onClick.AddListener(ToggleDeleteMode);
        UpdateButtonColor();
    }

    void ToggleDeleteMode()
    {
        isDeleteModeActive = !BuildManager.Instance.IsDeleteMode();
        BuildManager.Instance.SetDeleteMode(isDeleteModeActive);
        UpdateButtonColor();

        Debug.Log(isDeleteModeActive ? "🗑️ Mode suppression ACTIVÉ." : "❌ Mode suppression DÉSACTIVÉ.");
    }

    void UpdateButtonColor()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.color = isDeleteModeActive ? activeColor : normalColor;
        }
    }


    public void ResetButtonColor()
{
    isDeleteModeActive = false;
    UpdateButtonColor();
}

}
