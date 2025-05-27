using UnityEngine;
using UnityEngine.Tilemaps;

public class PNJMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float moveInterval = 2f;

    private Vector3 targetPosition;
    private Vector2 moveDirection;
    private bool isMoving = false;
    private float moveTimer;

    public Tilemap obstacleMap;
    public Animator animator;

    // ✅ Sprites pour la direction
    public Sprite spriteGauche;
    public Sprite spriteDroite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        moveTimer = moveInterval;
        targetPosition = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("❌ [PNJMovement] Aucun SpriteRenderer trouvé sur " + gameObject.name);
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            moveTimer -= Time.deltaTime;

            if (moveTimer <= 0f)
            {
                ChooseNewDirection();
                moveTimer = moveInterval;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
                UpdateAnimation(Vector2.zero);
            }
        }
    }

    void ChooseNewDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        for (int i = 0; i < 10; i++)
        {
            Vector2 newDirection = directions[Random.Range(0, directions.Length)];
            Vector3Int cell = obstacleMap.WorldToCell(transform.position + (Vector3)newDirection);
            TileBase tile = obstacleMap.GetTile(cell);

            if (tile == null)
            {
                moveDirection = newDirection;
                targetPosition = transform.position + (Vector3)moveDirection;
                isMoving = true;
                UpdateAnimation(moveDirection);
                return;
            }
        }

        UpdateAnimation(Vector2.zero);
    }

    void UpdateAnimation(Vector2 dir)
    {
        if (animator != null)
        {
            if (HasParameter(animator, "IsWalking"))
                animator.SetBool("IsWalking", dir != Vector2.zero);

            if (HasParameter(animator, "Direction"))
            {
                if (dir == Vector2.up) animator.SetInteger("Direction", 1);
                else if (dir == Vector2.down) animator.SetInteger("Direction", 0);
                else if (dir == Vector2.left) animator.SetInteger("Direction", 2);
                else if (dir == Vector2.right) animator.SetInteger("Direction", 3);
            }
        }

        // ✅ Gestion des sprites pour la direction gauche/droite
        if (spriteRenderer != null)
        {
            if (dir == Vector2.left && spriteGauche != null)
            {
                spriteRenderer.sprite = spriteGauche;
            }
            else if (dir == Vector2.right && spriteDroite != null)
            {
                spriteRenderer.sprite = spriteDroite;
            }
        }
    }

    bool HasParameter(Animator anim, string paramName)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}