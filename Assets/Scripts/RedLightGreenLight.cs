using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RedLightGreenLight : MonoBehaviour
{
    public float turnSpeed;

    public float intensityMultiplier = 1f;
    public int currentRound = 1;

    public bool isListening = false;

    public SpriteRenderer[] wallSprites;

    public Transform lookTarget;
    public ConeLOSScript coneLOSScript;

    private enum RedGreenState
    {
        Green,
        Red,
        Alert
    }
    private RedGreenState currentState = RedGreenState.Green;

    // Start is called before the first frame update
    void Start()
    {
        currentRound = 1;
        intensityMultiplier = 1f;

        coneLOSScript = GetComponent<ConeLOSScript>();

        lookTarget = GameObject.Find("Look_Target").transform;

        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Border_Walls");
        wallSprites = new SpriteRenderer[wallObjects.Length];

        for (int i = 0; i < wallObjects.Length; i++)
        {
            wallSprites[i] = wallObjects[i].GetComponent<SpriteRenderer>();
        }

        ChangeState(RedGreenState.Green);
    }

    // Update is called once per frame
    void Update()
    {
        RotateToTarget();
    }

    private void ChangeState(RedGreenState newState)
    {
        StopAllCoroutines();
        //Debug.Log("Stopping Coroutines");
        currentState = newState;
        switch (currentState)
        {
            case RedGreenState.Green:
                ChangeWallColor("#00FF00");
                coneLOSScript.ChangeConeColor("#400000");
                isListening = false;
                ResetPlayerHeat();
                LookAt(30, 0, 0);
                StartCoroutine(GreenState());
                break;

            case RedGreenState.Red:
                ChangeWallColor("#FF0000");
                coneLOSScript.ChangeConeColor("#400000");
                isListening = true;
                LookAt(20, 0, 0);
                StartCoroutine(RedState());
                break;
                
            case RedGreenState.Alert:
                ChangeWallColor("#FF0000");
                coneLOSScript.ChangeConeColor("#C80028");
                StartCoroutine(AlertState());
                break;
        }
    }

    void ChangeWallColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            foreach (SpriteRenderer wall in wallSprites)
            {
                wall.color = newColor;
            }
        }
    }

    void ResetPlayerHeat()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            // Get the PlayerScript attached to the player
            PlayerScript playerScript = player.GetComponent<PlayerScript>();

            if (playerScript != null)
            {
                // Change the Heat value
                playerScript.heat = 0;
            }
        }
    }

    void RotateToTarget()
    {
        Vector3 direction = (lookTarget.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void LookAt(float x, float y, float z)
    {
        lookTarget.transform.position = new Vector3(x, y, z);
    }

    IEnumerator GreenState()
    {
        turnSpeed = 300;
        while (true)
        {

            yield return new WaitForSeconds(Random.Range(2f, 5f));
            ChangeState(RedGreenState.Red);
        }
    }

    IEnumerator RedState()
    {
        while (true)
        {

            yield return new WaitForSeconds(Random.Range(4f, 7f));
            //LookAt(30, 0, 0);
            yield return new WaitForSeconds(1);

            //ChangeState(RedGreenState.Green);
        }
    }

    IEnumerator AlertState()
    {
        while (true)
        {
            yield break;
        }
    }
}