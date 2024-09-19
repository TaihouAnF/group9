using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManagerScript : MonoBehaviour
{

    ///Variables//////////////////////
    [Tooltip("Starting mood value of the passenger when the player starts the game, should be lower rather than higher")]
    [SerializeField]
    private int startingMood;

    [Tooltip("Maximum mood value of the passenger")]
    [SerializeField]
    private int maximumMood;

    //Mood of the passenger
    //Goes between 0 - maximumMood
    //Lower is bad, higher is good
    private int currentMood;

    //Pointer to the LightHouseManager in the scene
    private LightHouseManager lighthouseManager;
    
    //Pointer to the game manager in the scene
    private GameManager gameManager;

    //Pointer to the dialogue manager
    private DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        //Set the lhm to the lighthousemanager in the scene
        lighthouseManager = FindObjectOfType<LightHouseManager>();

        //Set the game manager to the one in the scene
        gameManager = FindObjectOfType<GameManager>();

        //Set the dialogue manager to the one in the scene
        dialogueManager = FindObjectOfType<DialogueManager>();

        //Set the currentMood to the startingMood
        currentMood = startingMood;
    } 

    public void AdjustMood(int value)
    {
        //Add the value to this currentMood
        currentMood += value;
        
        //Clamp the value so it can't go over the maximum set OR below 0
        Mathf.Clamp(currentMood, 0, maximumMood);

        //Since the story needs all the nodes to run out, the lighthouse should only travel proportionally each step
        float distance = gameManager.GetStartingDistance() / dialogueManager.GetNumNodes();

        //Behaviors that will trigger off the adjusted mood
        //If the reaction as positive
        if(value > 0)
        {
            //Move distance towards the player
            //lighthouseManager.MoveTowardsTarget(distance);
            lighthouseManager.ShouldMove(distance, true);
        }
        //If it was a negative reaction
        else
        {
            //Move distance away from the player
            //lighthouseManager.MoveAwayFromTarget(distance);
        }

        //Debug statement for evaluating the passenger's mood
        Debug.Log("Passenger Manager: CurrentMood = " + currentMood);
    }
}
