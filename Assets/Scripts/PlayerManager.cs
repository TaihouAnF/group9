using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Attributes")]
    public float playerRotateSpeed;
    [SerializeField]
    private float vesselRotateSpeed;
    [Tooltip("Tells Whether the player is in control")]
    [SerializeField]
    private bool controlling;
    [SerializeField]
    private bool hasDirection;
    [SerializeField]
    private int direction;
    private readonly int[] dir = { -1, 1 };
    private float horizontalInput;

    [Tooltip("Abs of the angle that the player must be less than on the Y axis to count as charging since they'll be facing the lighthouse")]
    [SerializeField]
    private float angleCheck;

    [Tooltip("The value that determines in seconds how long the player has to look at the lighthouse before being prompted to answer a dialogue")]
    [SerializeField]
    private float dialogueChargeTime;
    private float currentChargeTime;
    public float moveCd;
    private float currMoveCd;

    private DialogueManager dialogueManager;
    private GameManager gameManager;
    private LightHouseManager lightHouseManager;
    private WrappingHorizonScript horizon;

    [SerializeField]
    private GameObject lightHouse;
    [SerializeField]
    private float threshold;
    private Vector3 facingDir;
    [SerializeField]
    private float decisionTime;
    private float currDesTime;

    void Start()
    {

        currentChargeTime = 0;

        dialogueManager = FindObjectOfType<DialogueManager>();
        gameManager = FindObjectOfType<GameManager>();
        lightHouseManager = FindObjectOfType<LightHouseManager>();
        horizon = FindObjectOfType<WrappingHorizonScript>();

        controlling = false;
        hasDirection = false;

        currMoveCd = moveCd;
        facingDir = (lightHouse.transform.position - transform.position).normalized; 
    }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0) 
        {
            controlling = true;
            hasDirection = false;
        }
        else if (!controlling && !hasDirection)
        {
            hasDirection = true;
            var rng = Random.Range(0, 2);
            direction = dir[rng];
        }
        else
        {
            controlling = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        //Only update the rotation if the game state is active
        if (gameManager.GetGameState() == GameState.ACTIVE)
        {
            UpdatePlayerRotation();
            CheckCharge();
        }
        else if (gameManager.GetGameState() == GameState.ASKING) 
        {
            UpdatePlayerRotation();
            CheckDirection();
        }
    }

    /// <summary>
    ///  Controlling rotation of the player.
    /// </summary>
    private void UpdatePlayerRotation() 
    {
        if (horizontalInput != 0)
        {
            float rotationAmount = horizontalInput * playerRotateSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
            currMoveCd = moveCd;
        }
        else if (!controlling && currMoveCd <= 0)
        {
            float rotationAmount = direction * vesselRotateSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
        }
        else 
        {
            currMoveCd -= Time.deltaTime;
        }
    }

    private void CheckDirection()
    {
        float align = Vector3.Dot(transform.forward, facingDir);
        Debug.Log(align);
        if (gameManager.GetGameState() == GameState.ASKING && ((align > 0 && align > threshold) || (align < 0 && align < -1 + threshold))) // The player is facing the lighthouse
        {
            Debug.Log("The player should make a decision now.");
            // TODO: the player is facing the lighthouse/the edge
            currDesTime += Time.deltaTime;
            // TODO: make a slide show decision time.
            if (currDesTime >= decisionTime)
            {
                if (align > 0) 
                {
                    // TODO The player chose lighthouse
                    Debug.Log("The player chose lighthouse.");
                }
                else
                {
                    // TODO The player chose edge
                    Debug.Log("The player chose edge.");
                }
                ResetCharge();
            }
        }
        else 
        {
            currDesTime = 0;
        }
    }

    private void CheckCharge()
    {
        //Only count the charge when the game state is ACTIVE not paused
        if(gameManager.GetGameState() == GameState.ACTIVE)
        {
            currentChargeTime += Time.deltaTime;
            if(currentChargeTime >= dialogueChargeTime)
            {
                Debug.Log("PlayerManager: Should be starting a dialogue because charge has been met");
                // dialogueManager.StartDialogue();
                gameManager.SetGameState(GameState.ASKING);
            }
        }
    }

    public void ResetCharge()
    {
        currentChargeTime = 0;
        gameManager.SetGameState(GameState.ACTIVE);
    }

    /// <summary>
    /// Rotate the vessel by a curtain amount.(Deprecated)
    /// </summary>
    /// <param name="rotationAmount">The Amount for the rotation.</param>
    // private void RotateVessel(float rotationAmount) 
    // {
    //     currRotation += rotationAmount;
    //     currRotation = Mathf.Clamp(currRotation, -10, 10);
    //     if ((currRotation == -10 && direction == -1) || (currRotation == 10 && direction == -1)) 
    //     {
    //         hasDirection = true;
    //         direction = -direction;
    //     }
    //     transform.rotation = Quaternion.Euler(0f, currRotation, 0f);
    // }
}
