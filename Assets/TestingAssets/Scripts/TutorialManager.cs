using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public DoorTeleporter door1;
    public DoorTeleporter door2;
    public PlayerController playerController;
    public EnemyController enemyController;
    public GameObject hallwayTrigger;
    public PlayerHealthUI playerHealthUI; // Add this reference

    [Range(1, 18)]
    public int progressCounter = 1;
    //1 = start tutorial
    //2 = during intro dialouge
    //3 = after intro dialouge
    //4 = entering hallwau
    //5 = hallway dialouge
    //6 = hallway complete and start of hallway dialouge 2
    //7 = end of hallway dialouge
    //8 = enter arena
    //9 = arena dialouge
    //10 = fight
    //11 = fight 1 complete and start of arena dialouge2
    //12 = during arena dialouge 2
    //13 = fight 2 start
    //14 = fight 2 end and start of dialouge3
    //15 = end tutorial

    void Start()
    {
        // Find health UI if not assigned
        if (playerHealthUI == null)
        {
            playerHealthUI = FindObjectOfType<PlayerHealthUI>();
        }

        // Start with health bar hidden
        if (playerHealthUI != null)
        {
            playerHealthUI.Hide();
        }
    }

    void Update()
    {
        switch (progressCounter)
        {
            case 1:
                disableMovement(15f);
                progressCounter++;
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14:
                break;
            case 15:
                break;
        }
    }

    void disableMovement(float seconds)
    {
        playerController.canMove = false;
        StartCoroutine(wait(seconds));
    }

    IEnumerator wait(float seconds)
    {
        Debug.Log("Wait started at: " + Time.time);
        yield return new WaitForSeconds(seconds);
        Debug.Log("Wait ended: " + Time.time);
        playerController.canMove = true;
    }
}