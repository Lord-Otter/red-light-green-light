using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeLOSScript : MonoBehaviour
{
    public float visionRadius = 5f;
    [Range(0.1f, 360f)] public float visionAngle = 45f;
    public float newAngleSpeed = 5f;
    public float newAngle = 15f;

    public bool isFocusing = false;
    public Vector3 lookTargetPosition;
    public GameObject Look_Target;

    public LayerMask targetLayer;
    public LayerMask obstructionLayer;

    [Header("Cone Mesh Settings")]
    [SerializeField] private Material coneMaterial;
    [SerializeField] private int vertexCount = 30;
    public Color coneColor;

    private MeshFilter coneMeshFilter;
    private Mesh coneMesh;
    public MeshRenderer coneMeshRenderer;

    public List<GameObject> visibleTargets = new List<GameObject>();


    public float baseLength = 10f;
    public float distanceToBase = 5f;

    public bool canSeePlayer { get; private set; }

    void Start()
    {
        coneMeshFilter = new GameObject("VisionCone").AddComponent<MeshFilter>();
        coneMeshRenderer = coneMeshFilter.gameObject.AddComponent<MeshRenderer>();

        coneMeshRenderer.material = coneMaterial != null ? coneMaterial : new Material(Shader.Find("Sprites/Default"));

        coneMesh = new Mesh();
        coneMeshFilter.mesh = coneMesh;
    }

    void Update()
    {
        FOV();
        TransitionVisionAngle(newAngle, newAngleSpeed);
        MoveTarget(lookTargetPosition);
        DrawFOV();
        if (isFocusing)
        {
            DynamicVisionAngle();
        }
    }

    public void DynamicVisionAngle()
    {
        Vector2 directionToTarget = (Look_Target.transform.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, Look_Target.transform.position);

        float targetWidth = Look_Target.GetComponent<SpriteRenderer>().bounds.size.x;

        if (distanceToTarget > 0)
        {
            newAngle = 2 * Mathf.Atan(targetWidth / (2 * distanceToTarget)) * Mathf.Rad2Deg;
        }

        newAngle = Mathf.Clamp(newAngle, 0f, 360f);
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

    public void MoveTarget(Vector3 newTarget)
    {
        // Implement target movement logic if needed
    }

    private void DrawFOV()
    {
        Vector3[] vertices = new Vector3[vertexCount + 1];
        int[] triangles = new int[(vertexCount - 1) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount; i++)
        {
            float angle = Mathf.Lerp(-visionAngle / 2, visionAngle / 2, (float)i / (vertexCount - 1)) * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * visionRadius;
        }

        int triangleIndex = 0;
        for (int i = 1; i < vertexCount; i++)
        {
            triangles[triangleIndex++] = 0;
            triangles[triangleIndex++] = i;
            triangles[triangleIndex++] = (i + 1) % vertexCount + 1;
        }

        coneMesh.vertices = vertices;
        coneMesh.triangles = triangles;
        coneMesh.RecalculateNormals();

        coneMeshFilter.transform.position = transform.position;
        coneMeshFilter.transform.rotation = transform.rotation;

        //coneMeshRenderer.material.color = coneColor;

        if (visibleTargets.Count > 0)
        {
            canSeePlayer = true;
            coneMeshRenderer.sortingOrder = -1;
        }
        else
        {
            canSeePlayer = false;
            coneMeshRenderer.sortingOrder = -1;
        }
    }

    public void ChangeConeColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            coneMeshRenderer.material.color  = newColor;           
        }
    }

    private void TransitionVisionAngle(float newVisionAngle, float speed)
    {
        float angleDifference = Mathf.Abs(newVisionAngle - visionAngle);

        float t = Mathf.Clamp01(angleDifference / 360f);
        float easedSpeed = speed * (1 - Mathf.Pow(t, 3));

        if (visionAngle < newVisionAngle)
        {
            visionAngle += easedSpeed;
            if (newVisionAngle - visionAngle < easedSpeed)
            {
                visionAngle = newVisionAngle;
            }
        }

        else if (visionAngle > newVisionAngle)
        {
            visionAngle -= easedSpeed;
            if (visionAngle - newVisionAngle < easedSpeed)
            {
                visionAngle = newVisionAngle;
            }
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
