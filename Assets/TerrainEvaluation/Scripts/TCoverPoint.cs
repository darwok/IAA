using Unity.VisualScripting;
using UnityEngine;

public class TCoverPoint : MonoBehaviour
{
    public float coverValue; // 0-1
    public Transform player;
    public float maxCheckDistance = 50f;

    private void Update()
    {
        EvaluateCover();
    }

    void EvaluateCover()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.position).normalized;
        
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, maxCheckDistance))
        {
            if (hit.transform == player)
            {
                // Plyaer visible
                coverValue = 0f;
            }
            else
            {
                // Visibility blocked
                coverValue = 1f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;
        Gizmos.color = (coverValue > 0.5f) ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, player.position);
    }
}
