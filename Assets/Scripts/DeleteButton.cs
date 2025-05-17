using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ToggleDeleteMode);
    }

    void ToggleDeleteMode()
    {
        bool isActive = !BuildManager.Instance.IsDeleteMode();
        BuildManager.Instance.SetDeleteMode(isActive);

        Debug.Log(isActive ? "Mode suppression ACTIVÉ." : "Mode suppression DÉSACTIVÉ.");
    }
}
