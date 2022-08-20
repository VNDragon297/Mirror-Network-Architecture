using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : PlayerComponent
{
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask layerMask;

    private CharacterController charController;
    private PlayerInput playerInput;

    public float walkSpeedMultiplier = 1.0f;
    public float jumpHeight = 5.0f;
    public float mouseSens = 1.0f;
    public float gravity = -9.81f;

    [SyncVar] private PlayerInput.NetworkInputData Inputs;
    [SyncVar] private Vector3 moveDirection;
    [SyncVar] private Vector2 lookDelta;
    [SyncVar] private bool fired;
    [SyncVar(hook = nameof(OnWalkingCallback))] [SerializeField] private Vector2 velocity;
    [SyncVar(hook = nameof(OnAirbornCallback))] [SerializeField] private bool isGrounded;
    public Action<bool, string> onBooleanChanged;
    public Action<float, string> onFloatChanged;

    private void OnWalkingCallback(Vector2 oldVal, Vector2 newVal)
    {
        velocity = newVal;
        onFloatChanged?.Invoke(newVal.x, "velocityX");
        onFloatChanged?.Invoke(newVal.y, "velocityY");
    }

    private void OnAirbornCallback(bool oldVal, bool newVal)
    {
        isGrounded = newVal;
        onBooleanChanged?.Invoke(newVal, "isGrounded");
    }

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
        if (!hasAuthority)
            return;
        Inputs = playerInput.Inputs;

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
        var inputRemap = MoveAxisRemap(Inputs.moveDirection);
        moveDirection = new Vector3(inputRemap.x, moveDirection.y, inputRemap.z);

        if (Inputs.isRunPressed && moveDirection.z > 0f)
            moveDirection.z *= 2f;

        CmdPlayerMove(moveDirection);
    }

    [Command]
    private void CmdPlayerMove(Vector3 move)
    {
        // Setting moveDirection in server side
        moveDirection = move;
        // Checking for local direction before rotating the Vector
        CheckState();

        moveDirection = transform.right * moveDirection.x + transform.forward * moveDirection.z;

        if (isGrounded && moveDirection.y <= 0f && moveDirection.y > -2f)
            moveDirection.y = -2f;
        moveDirection.y += gravity;
        charController.Move(moveDirection * walkSpeedMultiplier * Time.fixedDeltaTime);
    }

    private void CheckState()
    {
        isGrounded = charController.isGrounded;

        if (isGrounded)
            velocity = new Vector2(moveDirection.x, moveDirection.z);
    }

    [Header("Camera Position")]
    public Transform headTransform;
    [SyncVar] float xRotation = 0f;
    private void Look()
    {
        // Using runner.Deltatime might be bad unless you have client prediction
        float mouseX = Inputs.lookDelta.x * mouseSens * Time.deltaTime;
        float mouseY = Inputs.lookDelta.y * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        CmdPlayerLook(mouseX, xRotation);
    }

    [Command]
    private void CmdPlayerLook(float mouseX, float xRot)
    {
        transform.Rotate(mouseX * Vector3.up);
        headTransform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
}
