using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceInfoDisplay : MonoBehaviour
{
    public GameObject buildManager;
    public TextMeshProUGUI resourceInfoText;

    public Image foodIcon;
    public TextMeshProUGUI foodAmount;
    public Image woodIcon;
    public TextMeshProUGUI woodAmount;
    public Image stoneIcon;
    public TextMeshProUGUI stoneAmount;
    public Image ironIcon;
    public TextMeshProUGUI ironAmount;
    public Image goldIcon;
    public TextMeshProUGUI goldAmount;

    void Start()
    {
        ResetIcons(); // Cache tout au départ
        if (resourceInfoText != null) resourceInfoText.gameObject.SetActive(false); // Rend le texte invisible au départ
    }

    void Update()
    {
        if (buildManager != null)
        {
            BuildManager manager = buildManager.GetComponent<BuildManager>();

            if (manager != null && manager.HasSelectedPrefab())
            {
                GameObject prefab = manager.GetSelectedPrefab();
                BuildingCost cost = prefab.GetComponent<BuildingCost>();

                if (cost != null)
                {
                    UpdateResource(foodIcon, foodAmount, cost.foodCost);
                    UpdateResource(woodIcon, woodAmount, cost.woodCost);
                    UpdateResource(stoneIcon, stoneAmount, cost.stoneCost);
                    UpdateResource(ironIcon, ironAmount, cost.ironCost);
                    UpdateResource(goldIcon, goldAmount, cost.goldCost);

                    if (resourceInfoText != null)
                        resourceInfoText.gameObject.SetActive(true); // Active le texte "Coût :"
                }
            }
            else
            {
                ResetIcons();
                if (resourceInfoText != null)
                    resourceInfoText.gameObject.SetActive(false); // Rend le texte invisible
            }
        }
    }

    public void DisplayCost(BuildingCost cost)
    {
        if (cost != null)
        {
            UpdateResource(foodIcon, foodAmount, cost.foodCost);
            UpdateResource(woodIcon, woodAmount, cost.woodCost);
            UpdateResource(stoneIcon, stoneAmount, cost.stoneCost);
            UpdateResource(ironIcon, ironAmount, cost.ironCost);
            UpdateResource(goldIcon, goldAmount, cost.goldCost);

            if (resourceInfoText != null)
                resourceInfoText.gameObject.SetActive(true); // Active le texte "Coût :"
        }
        else
        {
            ResetIcons();
            if (resourceInfoText != null)
                resourceInfoText.gameObject.SetActive(false); // Rend le texte invisible
        }
    }

    void UpdateResource(Image icon, TextMeshProUGUI amountText, int amount)
    {
        if (icon != null && amountText != null)
        {
            bool show = (amount > 0);
            icon.enabled = show;
            amountText.text = show ? amount.ToString() : "";
        }
    }

    void ResetIcons()
    {
        DisableResource(foodIcon, foodAmount);
        DisableResource(woodIcon, woodAmount);
        DisableResource(stoneIcon, stoneAmount);
        DisableResource(ironIcon, ironAmount);
        DisableResource(goldIcon, goldAmount);
    }

    void DisableResource(Image icon, TextMeshProUGUI amountText)
    {
        if (icon != null) icon.enabled = false;
        if (amountText != null) amountText.text = "";
    }
}
