using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : GameBehaviour
{
    public PlayerClass playerInfo;
    public PlayerAbilities playerAbilities;


    public AbilityCardClass[] drawnAbilityCards;

    public AbilityCardClass selectedAbilityCard;
    [SerializeField] int currentAbilityIndex;
    public WeaponClass currentWeapon;

    bool buttonCooldown;
    bool hasAttacked;
    bool attackIsHeld;

    [SerializeField]
    Health healthScript;
    PlayerControls controls;

    GameObject groundCheck;

    public int playerNum;

    Animator anim;
    Vector3 movement;
    Vector3 direction;
    Vector3 lastMovement; //to have player look there

    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public CharacterController controller;

    private void Awake()
    {
        //_GM.event_EnteredCombatScene.AddListener(DrawAbilityCards);

        playerAbilities = GetComponent<PlayerAbilities>();
        rb = GetComponent<Rigidbody>();
        healthScript.InitilizeHealth(playerInfo.health, playerInfo.defence);

        healthScript.ApplyParalysisEvent.AddListener(ApplyParalysis);
        healthScript.ApplySlownessEvent.AddListener(ApplySlowness);

        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        controls = new PlayerControls();

        groundCheck = transform.Find("GroundCheck").gameObject;

    }

    // Start is called before the first frame update
    void Start()
    {

        drawnAbilityCards = new AbilityCardClass[3];

        currentAbilityIndex = 0;

        selectedAbilityCard = drawnAbilityCards[0];

        _UI.combatUI.DisplayAbilityDecks();
        //test
        print("Draw cards");

        playerInfo = _UI.titleScreenUI.combatStyleStats.mage_stats;

        playerInfo.abilityDeck = _UI.titleScreenUI.abilityDecks.mage_AbilityDeck;

        DrawAbilityCards();

        transform.position = _GM.spawnPoints[playerNum].transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) { DrawAbilityCards(); }


        anim.SetFloat("WalkSpeed", controller.velocity.magnitude);

        //gravity check
        if(!Physics.CheckSphere(groundCheck.transform.position, 1, _GM.groundMask))
        {
            movement += Physics.gravity;

        }

        controller.Move(movement * playerInfo.movSpeed * Time.deltaTime);
        if (transform.localEulerAngles != Vector3.zero) direction = transform.localEulerAngles;
        if (controller.velocity.magnitude < 1) transform.localEulerAngles = direction;

        #region Rotate Player

        #endregion

        #region Attack
        if (attackIsHeld)
        {
            if (currentWeapon != null)
            {
                if (!hasAttacked)
                {
                    hasAttacked = true;
                    anim.SetTrigger("Attack");
                    ExecuteAfterSeconds(playerInfo.atkSpd, () => hasAttacked = false);
                }
            }
        }


        #endregion


    }

    void ApplyParalysis(float duration)
    {
        print("Paralysed");
        //paralysis animation plays here

        controller.enabled = false;
        ExecuteAfterSeconds(duration, () => controller.enabled = true);
    }

    void ApplySlowness(float duration, float strength)
    {
        float speedBeforeSlow = playerInfo.movSpeed;

        playerInfo.movSpeed = playerInfo.movSpeed * strength;

        ExecuteAfterSeconds(duration, () => playerInfo.movSpeed = speedBeforeSlow);

    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed) attackIsHeld = true;
        if (context.canceled) attackIsHeld = false;

        
    }

    public void DestroyPlayer()
    {
        var mouse = (GetComponentInChildren<GamepadCursor>().cursorTransform.gameObject);
        Destroy(gameObject.transform.GetChild(0).gameObject);
        Destroy(mouse);
        Destroy(gameObject);
    }

    public void DrawAbilityCards()
    {
        

        for (int i = 0; i < 3; i++)
        {
            print(i);
            var index = Random.Range(0, playerInfo.abilityDeck.Length);
            print(index);
            drawnAbilityCards[i] = playerInfo.abilityDeck[index];
            print(drawnAbilityCards[i]);
        }

        switch (_GM.playerGameObjList.IndexOf(gameObject))
        {
            case 0:
                _UI.combatUI.UpdateP1AbilityCards(drawnAbilityCards); break;
        }
    }

    public void ChangeSelectedAbility(int direction)
    {
        if (!buttonCooldown)
        {
            buttonCooldown = true;
            currentAbilityIndex += direction;

            if (currentAbilityIndex > 2) { currentAbilityIndex = 0; }
            if (currentAbilityIndex < 0) { currentAbilityIndex = 2; }
            print("Current index is " + currentAbilityIndex);

            selectedAbilityCard = drawnAbilityCards[currentAbilityIndex];


            ExecuteAfterSeconds(0.5f, () => buttonCooldown = false);
        }
        _UI.combatUI.UpdateSelectedAbilityCard(playerNum, currentAbilityIndex);


    }

    private void OnEnable()
    {
        //transform.position = _GM.spawnPoints[playerNum].transform.position;

        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();

    }


    public void OnUseSelectedAbility()
    {

        playerAbilities.CallAbility(selectedAbilityCard, currentAbilityIndex);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        movement = new Vector3(move.x, 0, move.y);
        transform.rotation = Quaternion.LookRotation(movement);

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Vector2 direction = context.ReadValue<Vector2>();
        //Vector3 heading = new Vector3(direction.x, 0, direction.y);
 
        //transform.rotation = Quaternion.LookRotation(heading);


    }
}

