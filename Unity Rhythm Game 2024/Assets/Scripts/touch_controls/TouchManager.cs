using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class TouchManager : MonoBehaviour
{
    //Camera to convert screen touches to world positions
    [SerializeField] private Camera CameraMain; 
    //private GameObject player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float YoffSet;
    [SerializeField] private float XoffSet;

    // Store all press input actions (independent for each finger)
    private InputAction[] _inputActions ;
    // Store the positions of individual presses
    private Dictionary<string, InputAction> _inputActionPositions;
    // Store the Judgement button for each press
    private Dictionary<string, Collider2D> _inputActionButtons;

    // Delegate used to notify Judgement buttons of finger press
    public delegate void TouchAction(int buttonId);
    public delegate void ReleaseAction(int buttonId);
    public event TouchAction OnTouch;

    public event TouchAction OnHold;
    public event ReleaseAction OnHoldRelease;

    // Number of InputActions to make(number of separate touches to support)
    // Ex: NUM_ACTIONS = 3 means that 3 fingers can interaction with the screen separately
    private int NUM_ACTIONS = 3;

    private void Awake()
    {
        // Setup Camera
        if (CameraMain == null)
        {
            CameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        _inputActions = new InputAction[NUM_ACTIONS];
        _inputActionPositions = new Dictionary<string, InputAction>();
        _inputActionButtons = new Dictionary<string, Collider2D>();
        for(int i = 0; i < NUM_ACTIONS; i++)
        {
            // press action
            InputAction newAction = new InputAction($"TouchPress{i}", InputActionType.Button, $"<Touchscreen>/touch{i}/press", "tap,hold");
            // press position
            InputAction newActionPosition = new InputAction($"TouchPressPosition{i}", InputActionType.Value, 
                                            $"<Touchscreen>/touch{i}/position", expectedControlType:"Vector2");
            
            _inputActions[i] = newAction;
            _inputActionPositions[newAction.name] = newActionPosition;
        }
        // Add binding https://docs.unity3d.com/Packages/com.unity.inputsystem@1.2/api/UnityEngine.InputSystem.InputBinding.html#UnityEngine_InputSystem_InputBinding_interactions
    }

    // Enable all input actions
    private void OnEnable()
    {
        for(int i = 0; i < NUM_ACTIONS; i++)
        {
            _inputActions[i].started += TouchPressed;
            _inputActions[i].canceled += TouchCanceled;
            _inputActions[i].Enable();
            _inputActionPositions[_inputActions[i].name].Enable();
        }
    }

    // Disable all input actions
    private void OnDisable()
    {
        for(int i = 0; i < NUM_ACTIONS; i++)
        {
            _inputActions[i].started -= TouchPressed;
            _inputActions[i].canceled -= TouchCanceled;
            _inputActions[i].Disable();
            _inputActionPositions[_inputActions[i].name].Disable();
        }

    }

    // Convert a screen touch to world position
    private Collider2D ScreenToWorldPosition(Vector3 touchPosition)
    {
        //Convert touch position to position on the Camera screen.
        touchPosition = CameraMain.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0f));
        touchPosition = new Vector3(touchPosition.x, touchPosition.y, 0f);
        // Gather all objects that overlap the touch location and check for a Judgement button
        Collider2D[] collider = Physics2D.OverlapPointAll(touchPosition);
        return Array.Find(collider, element => element.gameObject.CompareTag("JudgementButton"));
    }

    private void TouchCanceled(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;
        //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.InputAction.CallbackContext.html#UnityEngine_InputSystem_InputAction_CallbackContext_duration
        // Check duration to discern TapInteraction and HitInteraction will call this function
        if (context.interaction is HoldInteraction)
        {
            RaiseHoldReleaseEvent(_inputActionButtons[actionName].gameObject);
        }
    }
 
    private void TouchPressed(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;
        // Get button at touch position
        Collider2D button = ScreenToWorldPosition(_inputActionPositions[actionName].ReadValue<Vector2>());
        if (!button){return;}
        RaiseTouchEvent(button.gameObject);
        if (context.interaction is HoldInteraction)
        {
            OnHold?.Invoke(button.gameObject.GetComponent<JudgementButton>().Id);
            _inputActionButtons[actionName] = button;
        }
    }

    // Notify Judgement buttons of a screen touch
    private void RaiseTouchEvent(GameObject buttonGameObject)
    {
        JudgementButton judgementButton = buttonGameObject.GetComponent<JudgementButton>();
        OnTouch?.Invoke(judgementButton.Id);
    }

    private void RaiseHoldReleaseEvent(GameObject buttonGameObject)
    {
        JudgementButton judgementButton = buttonGameObject.GetComponent<JudgementButton>();
        OnHoldRelease?.Invoke(judgementButton.Id);
    }

}
