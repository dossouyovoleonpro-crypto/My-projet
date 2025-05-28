using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ColliderSettings : MonoBehaviour
{
    public Vector2 customSize = Vector2.one;
    public Vector2 customOffset = Vector2.zero;

    private void Awake()  // Utilisation de Awake pour que le collider soit prêt dès l'instanciation
    {
        ApplyColliderSettings();
    }

    public void ApplyColliderSettings()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = customSize;
        collider.offset = customOffset;
    }
}
