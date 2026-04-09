using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 9f;
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float airControl = 0.8f;

    CharacterController controller;
    Vector3 velocity;

    PlayerInputActions input;
    Vector2 moveInput;
    float jumpBufferTime = 0.15f;
    float jumpBufferCounter;

    bool wasGrounded;

    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;
    public float crouchSpeedMultiplier = 0.6f;
    public float crouchTransitionSpeed = 10f;
    bool isCrouching;
    float currentHeight;
    public Transform cameraHolder;
    public float cameraCrouchOffset = -0.5f;

    Vector3 originalCenter;
    float originalCameraY;
    Vector3 camStartLocalPos;
    float lastYVelocity;




    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Jump.performed += _ => jumpBufferCounter = jumpBufferTime;

        input.Player.Crouch.started += _ => isCrouching = true;
        input.Player.Crouch.canceled += _ => isCrouching = false;

    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void Update()
    {
        jumpBufferCounter -= Time.deltaTime;

        lastYVelocity = velocity.y;

        Move();
        ApplyGravity();
        HandleCrouch();
        controller.Move(velocity * Time.deltaTime);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        standHeight = controller.height;
        crouchHeight = Mathf.Min(crouchHeight, standHeight);

        originalCenter = controller.center;

        originalCameraY = cameraHolder.localPosition.y;

        camStartLocalPos = Camera.main.transform.localPosition;

    }


    void Move()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move.Normalize();

        if (controller.isGrounded)
        {
            float speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
            velocity.x = move.x * speed;
            velocity.z = move.z * speed;

            if (jumpBufferCounter > 0f && velocity.y <= 0f)
            {
                velocity.y = jumpForce;
                jumpBufferCounter = 0f;
            }
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, move.x * moveSpeed, airControl);
            velocity.z = Mathf.Lerp(velocity.z, move.z * moveSpeed, airControl);
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (!wasGrounded)
                OnLand();

            if (velocity.y < 0f)
                velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        wasGrounded = controller.isGrounded;
    }

    void OnLand()
    {
        GameObject cam = Camera.main.gameObject;

        iTween.Stop(cam);

        float impact = Mathf.Clamp(-lastYVelocity * 0.03f, 0.12f, 0.35f);

        iTween.MoveBy(
            cam,
            iTween.Hash(
                "y", -impact,
                "time", 0.08f,
                "islocal", true,
                "easetype", iTween.EaseType.easeOutQuad,
                "oncomplete", "LandRecover"
            )
        );

        iTween.RotateBy(
            cam,
            iTween.Hash(
                "x", 0.02f,
                "time", 0.1f,
                "islocal", true,
                "easetype", iTween.EaseType.easeOutQuad
            )
        );
    }



    void LandRecover()
    {
        GameObject cam = Camera.main.gameObject;

        iTween.MoveTo(
            cam,
            iTween.Hash(
                "position", camStartLocalPos,
                "time", 0.14f,
                "islocal", true,
                "easetype", iTween.EaseType.easeOutExpo
            )
        );
    }



    void OnJump()
    {
        iTween.PunchPosition(
            Camera.main.gameObject,
            new Vector3(0f, -0.15f, 0f),
            0.2f
        );
    }



    void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : standHeight;

        if (!isCrouching && controller.height < standHeight)
        {
            Vector3 origin = transform.position + Vector3.up * controller.height;
            if (Physics.SphereCast(
                origin,
                controller.radius,
                Vector3.up,
                out _,
                standHeight - controller.height))
            {
                targetHeight = controller.height;
            }
        }

        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            Time.deltaTime * crouchTransitionSpeed
        );

        float heightDelta = controller.height - standHeight;
        controller.center = originalCenter + Vector3.up * (heightDelta / 2f);

        Vector3 camPos = cameraHolder.localPosition;
        float targetCamY = isCrouching
            ? originalCameraY + cameraCrouchOffset
            : originalCameraY;

        camPos.y = Mathf.Lerp(
            camPos.y,
            targetCamY,
            Time.deltaTime * crouchTransitionSpeed
        );

        cameraHolder.localPosition = camPos;
    }


}