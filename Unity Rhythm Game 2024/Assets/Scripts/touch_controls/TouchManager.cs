using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class TouchManager : MonoBehaviour
{

    public static TouchManager Instance;

    //Camera to base touches on
    [SerializeField] private Camera CameraMain; 
    //private GameObject player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float YoffSet;
    [SerializeField] private float XoffSet;
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
        if (Instance == null)
        {
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
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
        touchPressAction.started += TouchPressed;
        touchPressAction2.started += TouchPressedTwo;
    }

    private void OnDisable()
    {
        touchPressAction.started -= TouchPressed;
        touchPressAction2.started -= TouchPressedTwo;
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
        ScreenToWorldPosition(touchPositionAction2.ReadValue<Vector2>());
    }
 
    private void TouchPressed(InputAction.CallbackContext context)
    {
        ScreenToWorldPosition(touchPositionAction.ReadValue<Vector2>());        
    }
    protected virtual void RaiseTouchEvent()
    {
        OnTouch?.Invoke();
    }

}
