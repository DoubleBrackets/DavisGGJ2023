using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputExtensionMethods
{
    public static void SetActionCallbacks(this InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.started += callback;
        action.performed += callback;
        action.canceled += callback;
    }

    public static void RemoveActionCallbacks(this InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.started -= callback;
        action.performed -= callback;
        action.canceled -= callback;
    }
}

