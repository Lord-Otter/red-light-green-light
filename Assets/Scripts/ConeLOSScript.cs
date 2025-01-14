using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeLOSScript : MonoBehaviour
{
    public float visionRadius = 5f;
    [Range(0.1f, 360f)] public float visionAngle = 45;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;

    // List to track multiple visible targets
    public List<GameObject> visibleTargets = new List<GameObject>();

    public bool canSeePlayer { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FOVCheck());
    }

    private IEnumerator FOVCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOV();
        }
    }

    private void FOV()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, visionRadius, targetLayer);

        visibleTargets.Clear();

        if (rangeCheck.Length > 0)
        {
            foreach (Collider2D targetCollider in rangeCheck)
            {
                Transform target = targetCollider.transform;
                Vector2 directionToTarget = (target.position - transform.position).normalized;

                if (Vector2.Angle(transform.up, directionToTarget) < visionAngle / 2)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, target.position);

                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                    {
                        visibleTargets.Add(target.gameObject);
                    }
                }
            }
        }

        if (visibleTargets.Count == 0 && canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, visionRadius);

        Vector3 angle1 = DirectionFromAngle(-transform.eulerAngles.z, -visionAngle / 2);
        Vector3 angle2 = DirectionFromAngle(-transform.eulerAngles.z, visionAngle / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + angle1 * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + angle2 * visionRadius);

        if (visibleTargets.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (GameObject target in visibleTargets)
            {
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }
    }

    private Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}