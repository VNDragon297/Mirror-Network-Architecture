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

    public float walkSpeedMultiplier = 1.0f;
    public float jumpHeight = 5.0f;
    public float mouseSens = 1.0f;
    public float gravity = -9.81f;
    public Transform gunSlot;

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

        playerInput.OnBuyPressed += BuyWeapon;
        playerInput.OnDropPressed += DropWeapon;
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
        else if (GetComponent<PlayerEntity>().isDead)
            return;

        Inputs = playerInput.Inputs;

        Look();
        Move();
        PlayerAction();
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        // Initiallize things such as audio manager and loggers
    }

    private void BuyWeapon()
    {
        // Enable ShopUI here

        // PLACEHOLDER
        // Should be done on server only
        CmdBuyWeapon();
    }

    private void DropWeapon() => CmdDropWeapon();

    [Command]
    private void CmdBuyWeapon()
    {
        var obj = Instantiate(MyNetworkManager.instance.spawnPrefabs[2], Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(obj);
        currentGun = obj;
    }

    [Command]
    private void CmdDropWeapon() => currentGun = null;

    [SyncVar(hook = nameof(OnCurrentGunChanged))][SerializeField] private GameObject currentGun;
    private void OnCurrentGunChanged(GameObject oldVal, GameObject newVal)
    {
        if(oldVal != null)
        {
            if (oldVal.TryGetComponent<Collider>(out Collider oldCol))
            {
                Debug.Log("Dropping weapon");
                var rb = oldVal.GetComponent<Rigidbody>();
                oldCol.enabled = true;
                rb.isKinematic = false;
                rb.AddExplosionForce(2f, this.transform.position, 2f);
            }
            oldVal.transform.parent = null;
        }

        currentGun = newVal;
        if(newVal != null)
        {
            if (currentGun.TryGetComponent<Collider>(out Collider col))
            {
                var rb = currentGun.GetComponent<Rigidbody>();
                col.enabled = false;
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }

            currentGun.transform.SetParent(gunSlot);
            currentGun.transform.SetPositionAndRotation(gunSlot.position, gunSlot.rotation);
        }
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

    public float raycastDistance = 100f;
    private void PlayerAction()
    {
        Transform origin = Camera.main.transform;
        if (Inputs.isFirePressed)
        {
            if (currentGun != null && currentGun.TryGetComponent<GunBehaviour>(out GunBehaviour gunBehaviour))
            {
                gunBehaviour.AttemptingToFire(origin.position, origin.forward, raycastDistance, GetComponent<PlayerEntity>().myIndex);
            }
        }
    }

    private void OnDestroy()
    {
        playerInput.OnBuyPressed -= BuyWeapon;
        playerInput.OnDropPressed -= DropWeapon;
    }
}
