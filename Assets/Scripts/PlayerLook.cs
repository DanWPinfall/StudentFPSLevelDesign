using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 0.08f;
    public float strafeTilt = 4f;
    public float tiltSmooth = 0.12f;
    public float bobAmount = 0.03f;
    public float bobSpeed = 14f;

    float xRotation;
    float currentTilt;
    float tiltVelocity;

    Vector2 lookInput;
    Vector3 camStartLocalPos;

    PlayerInputActions input;

    [Header("Aim Zoom")]
    public float aimFOV = 55f;
    public float zoomSpeed = 12f;

    float defaultFOV;
    float targetFOV;
    bool isAiming;


    void Awake()
    {
        input = new PlayerInputActions();
    }

    void Start()
    {
        camStartLocalPos = transform.localPosition;

        defaultFOV = Camera.main.fieldOfView;
        targetFOV = defaultFOV;

    }



    void OnEnable()
    {
        input.Player.Enable();

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookInput = Vector2.zero;

        input.Player.Aim.performed += _ =>
        {
            isAiming = true;

            iTween.PunchPosition(
                Camera.main.gameObject,
                new Vector3(0f, 0f, -0.05f),
                0.1f
            );
        };

        input.Player.Aim.canceled += _ =>
        {
            isAiming = false;

            iTween.PunchPosition(
                Camera.main.gameObject,
                new Vector3(0f, 0f, 0.03f),
                0.08f
            );
        };

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void OnDisable()
    {
        input.Player.Disable();
    }

    void Update()
    {
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        float moveX = input.Player.Move.ReadValue<Vector2>().x;
        float targetTilt = -moveX * strafeTilt;
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmooth);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, currentTilt);
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    void LateUpdate()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();

        if (move.magnitude > 0.1f)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.localPosition = camStartLocalPos + transform.right * bob;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                camStartLocalPos,
                Time.deltaTime * 10f
            );
        }

        targetFOV = isAiming ? aimFOV : defaultFOV;

        Camera.main.fieldOfView = Mathf.Lerp(
            Camera.main.fieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );

    }




}
