
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "ProtagUserInputProvider", menuName = "Architecture/Input/Providers/ProtagUserInputProvider")]

public class ProtagUserInputProvider : ProtagInputProvider
{
    [Header("Gameplay Actions")]
    
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference dash;
    [SerializeField] private InputActionReference primaryFire;
    [SerializeField] private InputActionReference secondaryFire;
    [SerializeField] private InputActionReference mousePosition;
    [SerializeField] private InputActionReference interact;
    [SerializeField] private InputActionReference pause;
    
    private PlayerInputEvents events;
    public override PlayerInputEvents Events => events;

    private PlayerInputState _state;

    private void OnEnable()
    {
        events = new PlayerInputEvents();
        // Link up events with unity Input System
        movement.action.SetActionCallbacks(OnMovement);
        mousePosition.action.SetActionCallbacks(OnMousePosition);
        dash.action.SetActionCallbacks(OnDash);
        pause.action.SetActionCallbacks(OnPause);
        interact.action.SetActionCallbacks(OnInteract);
        primaryFire.action.SetActionCallbacks(OnPrimaryFire);
        secondaryFire.action.SetActionCallbacks(OnSecondaryFire);
    }

    public override void GetInputState(ref PlayerInputState state)
    {
        state.movementVector = movement.action.ReadValue<Vector2>();
        state.mousePosition = mousePosition.action.ReadValue<Vector2>();
        state.dashHeld = dash.action.ReadValue<float>() > 0;
        state.dashPressed = dash.action.WasPerformedThisFrame();
    }

    // Callback Listeners

    private void OnMovement(InputAction.CallbackContext context)
    {
    }

    private void OnMousePosition(InputAction.CallbackContext context)
    {
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            events.OnDashPressed?.Invoke();
        if (context.canceled)
            events.OnDashReleased?.Invoke();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
            events.OnPausePressed?.Invoke();
    }


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
            events.OnInteractPressed?.Invoke();
    }


    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.started)
            events.OnPrimaryFirePressed?.Invoke();
    }


    public void OnSecondaryFire(InputAction.CallbackContext context)
    {
        if (context.started)
            events.OnSecondaryFirePressed?.Invoke();
    }
}

