using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerInput : PlayerComponent
{
    public struct NetworkInputData
    {
        public uint Buttons;
        public Vector2 moveDirection;
        public Vector2 lookDelta;

        public bool isUp(uint button) => isDown(button) == false;
        public bool isDown(uint button) => (Buttons & button) == button;

        public bool isFirePressed;
        public bool isJumpPressed;
    }

    public Gamepad gamepad;
    public NetworkInputData Inputs;

    [SerializeField] private InputAction Move;
    [SerializeField] private InputAction Look;
    [SerializeField] private InputAction Fire;
    [SerializeField] private InputAction Jump;

    private bool firePressed;
    private bool jumpPressed;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // Cloning player control if client gain authority of this
        Move.Clone();
        Look.Clone();
        Fire.Clone();
        Jump.Clone();

        EnableInputAction();

        Fire.performed += FirePressed;
        Jump.performed += JumpAction;
        Jump.canceled += JumpAction;
    }

    private void FirePressed(InputAction.CallbackContext ctx) => firePressed = true;
    private void JumpAction(InputAction.CallbackContext ctx) => jumpPressed = (ctx.performed) ? true : false;

    private void Update()
    {
        Inputs.moveDirection = Move.ReadValue<Vector2>();
        Inputs.lookDelta = Look.ReadValue<Vector2>();
    }

    public void EnableInputAction()
    {
        Move.Enable();
        Look.Enable();
        Fire.Enable();
        Jump.Enable();
    }

    public void DisableInputAction()
    {
        Move.Disable();
        Look.Disable();
        Fire.Disable();
        Jump.Disable();
    }

    // Dispose input will also unsubscribe input events
    private void DisposeInput()
    {
        Move.Dispose();
        Look.Dispose();
        Fire.Dispose();
        Jump.Dispose();
    }

    private void OnDestroy()
    {
        DisposeInput();
    }
}
