using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
using System.Collections; // IEnumerator()
using System.Collections.Generic;
public class TouchHold : MonoBehaviour
{
    private InputAction holdAction;

    [SerializeField] private PlayerInput playerInput;
    private List<int> activeTouches;
    private void Awake()
    {
        activeTouches = new List<int>();
        playerInput = GetComponent<PlayerInput>();
        // Create a new InputAction for touch hold
        holdAction = playerInput.actions["hold"];
        // holdAction = new InputAction("HoldAction", InputActionType.PassThrough);
        // holdAction.AddBinding("<Touchscreen>/press").WithInteraction("hold");
    }

    private void OnEnable()
    {
        // holdAction.Enable();
        holdAction.performed += OnHoldPerformed;
        holdAction.canceled += OnHoldCanceled;
    }

    private void OnDisable()
    {
        holdAction.performed -= OnHoldPerformed;
        holdAction.canceled -= OnHoldCanceled;
        // holdAction.Disable();
    }

    private void OnHoldPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            Debug.Log("tap");
        }   
        else
        {
            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    Vector2 touchPosition = touch.position.ReadValue();
                    Debug.Log($"Touch {touch.touchId.ReadValue()} Pressed at {touchPosition}");
                    activeTouches.Add(touch.touchId.ReadValue());
                }
            }
            Debug.Log("Holding...");
        }
    }

    private void OnHoldCanceled(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                int id = touch.touchId.ReadValue();
                if (activeTouches.Contains(id) && !touch.press.isPressed)
                {
                    activeTouches.Remove(id);
                    Debug.Log($"Touch {id} done");
                }
            }
        }
    }
}
