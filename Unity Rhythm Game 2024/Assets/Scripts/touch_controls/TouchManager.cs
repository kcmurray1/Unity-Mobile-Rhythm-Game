using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;


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

    // Delegate used to notify Judgement buttons of finger press
    public delegate void TouchAction(JudgementButton judgementButton);
    public event TouchAction OnTouch;

    // Number of InputActions to make
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
            Debug.Log(_inputActions[i]);
            _inputActions[i].performed += TouchPressed;
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
            _inputActions[i].performed -= TouchPressed;
            _inputActions[i].canceled -= TouchCanceled;
            _inputActions[i].Disable();
            _inputActionPositions[_inputActions[i].name].Disable();
        }

    }

    // Convert a screen touch to world position
    private Vector3 ScreenToWorldPosition(Vector3 touchPosition)
    {
        //Convert touch position to position on the Camera screen.
        touchPosition = CameraMain.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0f));
        touchPosition = new Vector3(touchPosition.x, touchPosition.y, 0f);
        // Gather all objects that overlap the touch location and check for a Judgement button
        Collider2D[] collider = Physics2D.OverlapPointAll(touchPosition);
        Collider2D button = Array.Find(collider, element => element.gameObject.CompareTag("JudgementButton"));
        if(button)
        {
            RaiseTouchEvent(button.gameObject);  
        }
        return touchPosition;
    }

    private void TouchCanceled(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            return;
        }
        //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.InputAction.CallbackContext.html#UnityEngine_InputSystem_InputAction_CallbackContext_duration
        else if (context.duration > 0.5){
            Debug.Log("Done holding!");
        }
    }
 
    private void TouchPressed(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            ScreenToWorldPosition(_inputActionPositions[context.action.name].ReadValue<Vector2>());
        }     
        else
        {
            Debug.Log($"{context.control.path} is holding...");
        
        }
    }

    // Notify Judgement buttons of a screen touch
    protected virtual void RaiseTouchEvent(GameObject buttonGameObject)
    {
        JudgementButton judgementButton = buttonGameObject.GetComponent<JudgementButton>();
        OnTouch?.Invoke(judgementButton);
    }

}
