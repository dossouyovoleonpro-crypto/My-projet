using UnityEngine;

public class FeuBoutton : MonoBehaviour
{
    public float radius = 10f;
    public LayerMask pnjLayer;

    // Ceci sera appelé par le bouton
    public void AlertPNJFire()
    {
        Collider2D[] pnjs = Physics2D.OverlapCircleAll(transform.position, radius, pnjLayer);

        foreach (Collider2D pnj in pnjs)
        {
            PNJMovement pnjMovement = pnj.GetComponent<PNJMovement>();
            if (pnjMovement != null)
            {
                pnjMovement.MoveToFire(transform.position);
            }
        }
    }

    // Pour afficher le rayon dans la scène
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
