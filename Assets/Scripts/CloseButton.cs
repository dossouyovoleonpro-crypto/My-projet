using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    public GameObject panelToClose;  // Référence explicite du panneau à fermer

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ClosePanel);
    }

    void ClosePanel()
    {
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
            Debug.Log($"❌ Fermeture du panneau : {panelToClose.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun panneau spécifié à fermer. Vérifiez le champ 'panelToClose'.");
        }
    }
}
