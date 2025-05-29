using UnityEngine;
using System.Collections;

public class CameraController2D : MonoBehaviour
{
    public float moveSpeed = 20f;  // vitesse de déplacement
    public float zoomSpeed = 10f;  // vitesse du zoom
    public float minZoom = 3f;
    public float maxZoom = 20f;

    private Camera cam;
    private Vector2 minBounds = new Vector2(-190, -100);
    private Vector2 maxBounds = new Vector2(190, 100);

    private Vector3 moveInput;

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(CameraControlCoroutine());  // Lance la coroutine
    }

    IEnumerator CameraControlCoroutine()
    {
        while (true)
        {
            // Lecture des entrées
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            moveInput = new Vector3(moveX, moveY, 0f);

            // Déplacement indépendant du timeScale
            float deltaTime = Time.unscaledDeltaTime;
            Vector3 newPosition = transform.position + moveInput * moveSpeed * deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
            transform.position = newPosition;

            // Gestion du zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                float newZoom = cam.orthographicSize - scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
            }

            // Attendre la prochaine frame, mais sans être affecté par timeScale
            yield return null;
        }
    }
}
