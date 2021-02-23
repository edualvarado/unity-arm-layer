using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


[Serializable]
public class MoveInputEvent : UnityEvent<float, float> { }

public class InputController : MonoBehaviour
{
    #region Variables

    public Actions controls;
    public MoveInputEvent moveInputEvent;

    #endregion


    private void Awake()
    {
        controls = new Actions();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += OnMovePerformed;
        controls.Gameplay.Move.canceled += OnMovePerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveInputEvent.Invoke(moveInput.x, moveInput.y);
        //Debug.Log($"Move Input: {moveInput}");
    }
}
