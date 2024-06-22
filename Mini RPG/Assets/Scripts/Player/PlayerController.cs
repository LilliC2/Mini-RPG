using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : GameBehaviour
{

    public enum PlayerState { Attacking, Falling, Idle, Moving}
    public PlayerState playerStae;
    [Header("Player Stats")]

    public PlayerClass playerInfo;
    public PlayerAbilities playerAbilities;
    [SerializeField] GameObject playerVisualPrefab;
    public int playerNum;


    [Header("Abilities")]

    public AbilityCardClass[] drawnAbilityCards;
    public AbilityCardClass selectedAbilityCard;


    [Header("Attacks and Weapon")]

    bool buttonCooldown;
    bool hasAttacked;
    bool attackIsHeld;
    [SerializeField] int currentAbilityIndex;
    public WeaponClass currentWeapon;
    [HideInInspector]
    public Health healthScript;

    [Header("Combos")]

    public int currentAttackCount;

    public float stepAmount;

    [Header("Movement")]

    PlayerControls controls;
    GameObject groundCheck;
    Animator anim;
    Vector3 movement;
    Vector3 direction;
    Vector3 lastMovement; //to have player look there
    [SerializeField] float rotateSpeed;
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

        //test
        print("Draw cards");

        playerInfo = _UI.titleScreenUI.combatStyleStats.mage_stats;

        playerInfo.abilityDeck = _UI.titleScreenUI.abilityDecks.mage_AbilityDeck;

        DrawAbilityCards();

        transform.position = _GM.spawnPoints[playerNum].transform.position;

        _UI.combatUI.UpdatePlayerHealthMana(playerNum, healthScript.currentHealth, healthScript.maxHealth, playerInfo.currentMana, playerInfo.maxMana);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            _UI.combatUI.UpdatePlayerHealthMana(playerNum, healthScript.currentHealth, healthScript.maxHealth, playerInfo.currentMana, playerInfo.maxMana);
        }


        anim.SetFloat("WalkSpeed", rb.velocity.magnitude);

        //gravity check
        if(!Physics.CheckSphere(groundCheck.transform.position, 1, _GM.groundMask))
        {
            //movement += Physics.gravity;

        }

        //controller.Move(transform.TransformDirection(( movement * playerInfo.movSpeed) * Time.deltaTime));

        transform.Translate(movement * playerInfo.movSpeed * Time.deltaTime, Space.World);
        if (transform.localEulerAngles != Vector3.zero) direction = transform.localEulerAngles;
        if (controller.velocity.magnitude < 1) transform.localEulerAngles = direction;

        #region Rotate Player
        if(movement != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotateSpeed * Time.deltaTime);
        }

        #endregion

        #region Attack

        anim.SetBool("Attacking", attackIsHeld);

        if (attackIsHeld)
        {
            
            AttackCombos();


        }




        #endregion


    }

    #region Debuff
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
    #endregion
    
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed) attackIsHeld = true;
        if (context.canceled) attackIsHeld = false;

    }

    void AttackCombos()
    {
        if(!hasAttacked)
        {
            hasAttacked = true;

            print(currentAttackCount);

            ExecuteAfterSeconds(playerInfo.atkSpd,()=> hasAttacked = false);
        }

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

        //switch (_GM.playerGameObjList.IndexOf(gameObject))
        //{
        //    case 0:
        //        _UI.combatUI.UpdateP1AbilityCards(drawnAbilityCards); break;
        //}
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
       // _UI.combatUI.UpdateSelectedAbilityCard(playerNum, currentAbilityIndex);


    }

    public void OnHit()
    {
        _UI.combatUI.UpdatePlayerHealthMana(playerNum, healthScript.currentHealth, healthScript.maxHealth, playerInfo.currentMana, playerInfo.maxMana);

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

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Vector2 direction = context.ReadValue<Vector2>();
        //Vector3 heading = new Vector3(direction.x, 0, direction.y);
 
        //transform.rotation = Quaternion.LookRotation(heading);


    }
}

