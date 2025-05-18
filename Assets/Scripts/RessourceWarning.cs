using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceWarning : MonoBehaviour
{
    public Text resourceText;
    public Color warningColor = Color.red;
    public float flashDuration = 0.5f;

    private Color originalColor;

    void Start()
    {
        if (resourceText == null)
            resourceText = GetComponent<Text>();

        originalColor = resourceText.color;
    }

    public void FlashWarning()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        for (int i = 0; i < 3; i++) // Clignote 3 fois
        {
            resourceText.color = warningColor;
            yield return new WaitForSeconds(flashDuration);
            resourceText.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}
