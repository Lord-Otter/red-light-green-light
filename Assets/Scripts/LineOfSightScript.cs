
using UnityEngine;

public class LineOfSightScript : MonoBehaviour
{
    private GameObject player;
    internal bool hasLineOfSight = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player1");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        RaycastToPlayer();
    }

    void RaycastToPlayer()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.CompareTag("Player");
            if (hasLineOfSight)
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
            }
        }

    }
}