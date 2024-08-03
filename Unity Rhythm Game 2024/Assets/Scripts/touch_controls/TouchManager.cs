using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class TouchManager : MonoBehaviour
{

    [SerializeField] private GameObject _JudgementLine;
    //Camera to base touches on
    [SerializeField] private Camera CameraMain; 
    //private GameObject player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float YoffSet;
    [SerializeField] private float XoffSet;

    [SerializeField] private GameObject TouchObject;

    //USED FOR DEBUGGING
    [SerializeField] private GameObject TouchObjectTwo;
    private InputAction touchPositionAction2;
    //END OF DEBUGGING
    //Action that receives the position of a touch
    private InputAction touchPositionAction;
    //Action that waits to receive touch
    private InputAction touchPressAction;

    private InputAction touchPressAction2;
    

    // Delegates
    public delegate void TouchAction();

    public event TouchAction OnTouch;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPositionAction2 = playerInput.actions["TouchPosition2"];
        touchPressAction2 = playerInput.actions["TouchPress2"];
    }
    private void OnEnable()
    {
        touchPressAction.started += TouchPressed;
        touchPressAction.canceled += ResetPosition;
        touchPressAction2.started += TouchPressedTwo;
        touchPressAction2.canceled += ResetPosition2;
    }

    private void OnDisable()
    {
        touchPressAction.started -= TouchPressed;
        touchPressAction.canceled -= ResetPosition;
        touchPressAction2.started -= TouchPressedTwo;
        touchPressAction2.canceled -= ResetPosition2;
    }
    private void ResetPosition(InputAction.CallbackContext context)
    {
        TouchObject.transform.position = new Vector3(0f, 0f, 0f);
    }
    private void ResetPosition2(InputAction.CallbackContext context)
    {
        TouchObjectTwo.transform.position = new Vector3(0f, 0f, 0f);
    }

    private Vector3 ScreenToWorldPosition(Vector3 touchPosition)
    {
        //Convert touch position to position on the Camera screen.
        touchPosition = CameraMain.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0f));
        touchPosition = new Vector3(touchPosition.x, touchPosition.y, 0f);
        Collider2D collider = Physics2D.OverlapPoint(touchPosition);
        if (collider != null && collider.gameObject.CompareTag("Note"))
        {
            RaiseTouchEvent();  
        }
        return touchPosition;
    }

    private void TouchPressedTwo(InputAction.CallbackContext context)
    {
        TouchObjectTwo.transform.position = ScreenToWorldPosition(touchPositionAction2.ReadValue<Vector2>());
    }
 
    private void TouchPressed(InputAction.CallbackContext context)
    {
        TouchObject.transform.position = ScreenToWorldPosition(touchPositionAction.ReadValue<Vector2>());    
    
    }
    protected virtual void RaiseTouchEvent()
    {
        OnTouch?.Invoke();
    }

}
