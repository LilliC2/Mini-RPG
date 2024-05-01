using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public enum GameState { PlayersInUI, Combat, Story, Shop, Rest, OverworldMap}
    public GameState gameState;

    public GameObject[] spawnPoints;
    public List<PlayerInput> playerInputList = new List<PlayerInput>();
    public List<GameObject> playerGameObjList = new List<GameObject>();

    //bind action
    public  InputAction joinAction;
    public InputAction leaveAction;

    public int mapCounter; //number of maps the players have traversed
    public LayerMask playerLayerMask;

    //events
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    public UnityEvent event_ChangeActionMap;
    public UnityEvent event_EnteredCombatScene;
    public UnityEvent event_newMap;
    public UnityEvent event_LoadedNewScene;


    //layer masks
    public LayerMask groundMask;
    public LayerMask playerMask;


    public void Awake()
    {
        mapCounter = 0;
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        joinAction.Enable();
        leaveAction.Enable();
        //subscribe method to action
        joinAction.performed += context => JoinAction(context); //pass context to JoinAction()
        leaveAction.performed += context => LeaveAction(context);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        //have player 1 auto join, no split screen
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Scene loaded");
        event_LoadedNewScene.Invoke();
        event_ChangeActionMap.Invoke();

        if (scene.name.Contains("Combat"))
        {
            event_EnteredCombatScene.Invoke();
           
        }
 

    }

    //Called from PlayerInputManager
    void OnPlayerJoined(PlayerInput playerInput)
    {
        _UI.ToggleCursors();    
        playerInputList.Add(playerInput);

        int index = playerInputList.IndexOf(playerInput);

        if (SceneManager.GetActiveScene().name == "Title")
        {
            _UI.titleScreenUI.PlayerJoinedGame(index);
        }
            if (SceneManager.GetActiveScene().name.Contains("Combat")) _UI.combatUI.DisplayAbilityDecks();


        if (PlayerJoinedGame != null) //check if anything is subscribed
        {
            PlayerJoinedGame(playerInput); //be able to send this to anything that wants it   
        }
    }
    //Called from PlayerInputManager
    void OnPlayerLeft(PlayerInput playerInput)
    {

    }

    void JoinAction(InputAction.CallbackContext context)
    {

        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context); //getting controller from button input

    }
    void LeaveAction(InputAction.CallbackContext context)
    {
        if(playerInputList.Count > 1) //make sure theres more than 1 player
        {
            foreach (var player in playerInputList)
            {
                //search for all devices registered to player
                foreach (var device in player.devices)
                {
                    //if device matches devices used to prompt leave action
                    if(device != null && context.control.device == device)
                    {
                        UnregisterPlayer(player);
                        return;
                    }
                }
            }
        }

    }

    void UnregisterPlayer(PlayerInput playerInput)
    {
        _UI.titleScreenUI.PlayerLeftGame(playerInputList.IndexOf(playerInput));

        playerInputList.Remove(playerInput);

        if(PlayerLeftGame != null) //check for listeners
        {
            PlayerLeftGame(playerInput);
        }

        playerInput.GetComponentInParent<PlayerController>().DestroyPlayer();
    }
}
