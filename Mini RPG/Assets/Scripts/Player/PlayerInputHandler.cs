using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;



public class PlayerInputHandler : GameBehaviour
{
    public GameObject playerPrefab;
    PlayerController playerControls;

    PlayerInput playerInput;

    Vector3 startPos = new Vector3(0, 0, 0);

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        _GM.event_ChangeActionMap.AddListener(ChangeActionMap);

        if (playerPrefab != null)
        {
            if(_GM.spawnPoints.Length == 0) _GM.spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            playerControls = GameObject.Instantiate(playerPrefab, _GM.spawnPoints[_GM.playerGameObjList.Count].transform.position, transform.rotation).GetComponent<PlayerController>();
            var go = playerControls.gameObject;
            _GM.playerGameObjList.Add(go);
            playerControls.playerNum = _GM.playerGameObjList.IndexOf(go);

            transform.parent = playerControls.transform;
            transform.position = playerControls.transform.position;

            //if in titlescreen, look at camera

        }


    }

    void ChangeActionMap()
    {
        switch (_GM.gameState)
        {
            case GameManager.GameState.PlayersInUI:
                playerInput.currentActionMap = playerInput.actions.FindActionMap("UI");

                break;
                
            case GameManager.GameState.OverworldMap:
                print("Change to UI");
                print(playerInput.name);
                playerInput.currentActionMap = playerInput.actions.FindActionMap("UI");

                break;

            default:
                playerInput.currentActionMap = playerInput.actions.FindActionMap("Gameplay");

                break;
        }
    }

    #region Combat Action Map
    public void Attack(InputAction.CallbackContext context)
    {
        playerControls.Attack(context);
    }

    public void SelectAbilityCardL()
    {
        playerControls.ChangeSelectedAbility(-1);
    }

    public void SelectAbilityCardR()
    {
        playerControls.ChangeSelectedAbility(+1);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        playerControls.OnLook(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        switch (_GM.gameState)
        {
            case GameManager.GameState.PlayersInUI:
                //mouseController.OnMove(context);

                break;
            case GameManager.GameState.Combat:
                playerControls.OnMove(context);

                break;
        }
    }

    #endregion

    #region UI Action Map
    public void SelectOptionL()
    {
        _UI.overworldMapUI.ChangePlayerOptionChoice(-1, _GM.playerGameObjList.IndexOf(transform.parent.gameObject));
    }
    
    public void SelectOptionR()
    {
        _UI.overworldMapUI.ChangePlayerOptionChoice(+1, _GM.playerGameObjList.IndexOf(transform.parent.gameObject));

    }

    public void Confirm()
    {
        _UI.overworldMapUI.ConfirmPlayerOptionChoice(_GM.playerGameObjList.IndexOf(transform.parent.gameObject));
    }

    public void Back()
    {
        _UI.overworldMapUI.RevokePlayerOptionChoice(_GM.playerGameObjList.IndexOf(transform.parent.gameObject));

    }

    #endregion



}
