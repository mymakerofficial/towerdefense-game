using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public GameObject nextCheckpoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.2f);
        if(nextCheckpoint) Gizmos.DrawLine(transform.position, nextCheckpoint.transform.position);
    }
}
