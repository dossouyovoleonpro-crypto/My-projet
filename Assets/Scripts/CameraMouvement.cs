using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    public float moveSpeed = 20f;  // vitesse de deplacement de la caméra 
    public float zoomSpeed = 10f;  // vitesse du zoom de la cam
    public float minZoom = 3f;     // zoom le plus proche possible des tiles
    public float maxZoom = 20f;    // zoom le plus éloigné possible des tiles

    private Camera cam;


    private Vector2 minBounds = new Vector2(-190, -100);     // limite en bas a gauche de la caméra
    private Vector2 maxBounds = new Vector2(190, 100);       // limite en haut a droite de la caméra

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Déplacement horizontal et vertical
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);
        
        // Appliquer les limites de la carte
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        transform.position = newPosition;

        // Zoom avec la molette de la souris
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newZoom = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }
}
