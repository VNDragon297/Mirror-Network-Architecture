using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : PlayerComponent
{
    private CharacterController charController;
    private PlayerInput playerInput;

    public float walkSpeed = 1.0f;
    public float mouseSens = 1.0f;

    [SyncVar] private PlayerInput.NetworkInputData Inputs;
    [SyncVar] private Vector3 moveDirection;
    [SyncVar] private Vector2 lookDelta;
    [SyncVar] private bool fired;
    [SyncVar(hook = nameof(OnVelocityChangedCallback))]
    private bool isWalking;
    [SyncVar(hook = nameof(OnPlayerAirborneCallback))]
    private bool isGrounded;
    public event Action<bool, string> OnBooleanChanged;

    private void OnVelocityChangedCallback(bool oldVal, bool newVal) => isWalking = newVal;

    private void OnPlayerAirborneCallback(bool oldVal, bool newVal) => isGrounded = newVal;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

    }

    private void Update()
    {
        if(hasAuthority)
        {
            if(Entity.input.gamepad != null)
            {
                // Setup gamepad here
            }
        }
    }

    private void FixedUpdate()
    {
        Inputs = playerInput.Inputs;
        isGrounded = charController.isGrounded;

        Look();
        Move();
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        // Initiallize things such as audio manager and loggers
    }

    private Vector3 MoveAxisRemap(Vector2 controllerInput) => new Vector3(controllerInput.x, 0, controllerInput.y);

    private void Move()
    {
        moveDirection = MoveAxisRemap(Inputs.moveDirection);
        Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.z;
        charController.Move(move * walkSpeed * Time.fixedDeltaTime);

        if (isGrounded)
            isWalking = (moveDirection.z >= .125f);
    }

    [Header("Camera Position")]
    public Transform headTransform;
    float xRotation = 0f;
    private void Look()
    {
        // Using runner.Deltatime might be bad unless you have client prediction
        float mouseX = Inputs.lookDelta.x * mouseSens * Time.deltaTime;
        float mouseY = Inputs.lookDelta.y * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Currently broken on clients
        transform.Rotate(mouseX * Vector3.up);
        headTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
