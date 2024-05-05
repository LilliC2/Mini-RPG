using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;


public class GamepadCursor : GameBehaviour
{
    [SerializeField]
    public PlayerInput playerInput;
    private Mouse virtualMouse;
    [SerializeField]
    public RectTransform cursorTransform;

    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] Canvas canvas;
    Camera camera;

    [SerializeField]
    float cursorSpeed = 1000;

    [SerializeField]
    float padding = 35f;

    Gamepad userGamepad;

    bool previousMouseState;

    private void Awake()
    {
        canvas = FindAnyObjectByType<Canvas>();

        _GM.event_newMap.AddListener(CreateCursor);

        canvasRectTransform = canvas.GetComponent<RectTransform>();
        camera = Camera.main;
    }

    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();


        canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvas = FindAnyObjectByType<Canvas>();
        camera = Camera.main;

        CreateCursor();

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        //set inital pos
        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        userGamepad = playerInput.GetDevice<Gamepad>();

        if(_GM.gameState == GameManager.GameState.PlayersInUI)
        {
            InputSystem.onAfterUpdate += UpdateMotion; //similar to late update

        }


    }

    void CreateCursor()
    {

        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            canvasRectTransform = canvas.GetComponent<RectTransform>();

            canvasRectTransform = canvas.GetComponent<RectTransform>();
            canvas = FindAnyObjectByType<Canvas>();
        }

        //print("create new cursor");
        if (cursorTransform == null)
        {
            cursorTransform = Instantiate(Resources.Load<GameObject>("Prefabs/Mouse"), canvas.transform).GetComponent<RectTransform>();

        }

 
    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateMotion;
        if(virtualMouse !=null && virtualMouse.added)InputSystem.RemoveDevice(virtualMouse);

    }


    //update mouse with device motion
    void UpdateMotion()
    {
        if(_GM.gameState == GameManager.GameState.PlayersInUI)
        {
            if (cursorTransform.gameObject.activeSelf)
            {

                if (virtualMouse == null || userGamepad == null)
                    return;

                #region Movement
                Vector2 stickValue = userGamepad.leftStick.ReadValue();
                stickValue *= cursorSpeed * Time.deltaTime;

                Vector2 currentPos = virtualMouse.position.ReadValue();
                Vector2 newPos = currentPos + stickValue;

                //clamp so it doesnt go out of screen
                newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width - padding);
                newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height - padding);

                InputState.Change(virtualMouse, newPos);
                InputState.Change(virtualMouse.delta, stickValue);
                #endregion

                #region Buttons

                bool aButtonIsPressed = userGamepad.aButton.IsPressed();
                if (previousMouseState != aButtonIsPressed)
                {
                    virtualMouse.CopyState<MouseState>(out var mouseState); //copy current state of mouse
                    mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
                    InputState.Change(virtualMouse, mouseState);
                    previousMouseState = aButtonIsPressed;
                }

                AnchorCursor(newPos);
                #endregion
            }
        }




    }

    /// <summary>
    /// Map screen co-ord position to a rect co-ord pos
    /// </summary>
    /// <param name="pos"></param>
    void AnchorCursor(Vector2 pos)
    {
        //have to adapt with scale
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, out anchoredPosition);

        cursorTransform.anchoredPosition = anchoredPosition; 
    }
}
