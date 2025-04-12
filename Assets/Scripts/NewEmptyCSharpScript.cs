using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;       // Vitesse de déplacement
    public float moveInterval = 2f;      // Temps entre chaque déplacement

    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        ChooseNewTarget();  // Choisir une première position
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Si le délai est passé, choisir une nouvelle position
        if (timer >= moveInterval)
        {
            ChooseNewTarget();
            timer = 0;
        }

        // Se déplacer vers la position cible
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void ChooseNewTarget()
    {
        // Choisit un petit déplacement aléatoire
        float dx = Random.Range(-1f, 1f);
        float dy = Random.Range(-1f, 1f);
        targetPosition = transform.position + new Vector3(dx, dy, 0);
    }
}
