using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotationOffset;

    private Transform player;
    private LineOfSightScript lineOfSightScript;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player1").transform;
        lineOfSightScript = GetComponent<LineOfSightScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lineOfSightScript.hasLineOfSight)
        {
            MoveTowardPlayer();
        }
    }

    void MoveTowardPlayer()
    {
        //Movement
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

        //Rotation
        Vector3 direction = player.position - transform.position;
        direction.z = 0;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += rotationOffset;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
