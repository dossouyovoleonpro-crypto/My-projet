using UnityEngine;

public class ConstructionPanelSwitcher : MonoBehaviour
{
    public GameObject panel1;  // FenetreConstruction1
    public GameObject panel2;  // FenetreConstruction2

    private bool showingPanel1 = true;

    public void TogglePanel()
    {
        showingPanel1 = !showingPanel1;

        panel1.SetActive(showingPanel1);
        panel2.SetActive(!showingPanel1);

        Debug.Log("Affiche " + (showingPanel1 ? "panel 1" : "panel 2"));
    }
}
