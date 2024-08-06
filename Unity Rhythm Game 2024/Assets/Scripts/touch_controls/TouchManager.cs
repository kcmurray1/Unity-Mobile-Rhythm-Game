using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;


public class TouchManager : MonoBehaviour
{
    //Camera to base touches on
    [SerializeField] private Camera CameraMain; 
    //private GameObject player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float YoffSet;
    [SerializeField] private float XoffSet;
    private InputAction touchPositionAction2;
    
    //Action that receives the position of a touch
    private InputAction touchPositionAction;
    //Action that waits to receive touch
    private InputAction touchPressAction;

    private InputAction touchPressAction2;
    

    // Delegates
    public delegate void TouchAction(JudgementButton judgementButton);

    public event TouchAction OnTouch;

    private void Awake()
    {
        if (CameraMain == null)
        {
            CameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPositionAction2 = playerInput.actions["TouchPosition2"];
        touchPressAction2 = playerInput.actions["TouchPress2"];

    }
    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
        touchPressAction.canceled += TouchCanceled;
        touchPressAction2.performed += TouchPressedTwo;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
        touchPressAction2.performed -= TouchPressedTwo;
        touchPressAction.canceled -= TouchCanceled;
    }

    private Vector3 ScreenToWorldPosition(Vector3 touchPosition)
    {
        //Convert touch position to position on the Camera screen.
        touchPosition = CameraMain.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0f));
        touchPosition = new Vector3(touchPosition.x, touchPosition.y, 0f);
        Collider2D[] collider = Physics2D.OverlapPointAll(touchPosition);
        var button = Array.Find(collider, element => element.gameObject.CompareTag("JudgementButton"));
       
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
    private void TouchPressedTwo(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            ScreenToWorldPosition(touchPositionAction2.ReadValue<Vector2>());
        }
       
    }
 
    private void TouchPressed(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
             ScreenToWorldPosition(touchPositionAction.ReadValue<Vector2>());    
        }     
    }
    protected virtual void RaiseTouchEvent(GameObject buttonGameObject)
    {
        JudgementButton judgementButton = buttonGameObject.GetComponent<JudgementButton>();
        OnTouch?.Invoke(judgementButton);
    }

}
