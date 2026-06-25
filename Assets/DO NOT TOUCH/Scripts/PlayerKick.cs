using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerKick : MonoBehaviour
{
    [Header("Kick Settings")]
    public float kickRange = 2.2f;
    public float kickRadius = 0.4f;
    public float kickForce = 12f;
    public float kickCooldown = 0.4f;

    [Header("Layers")]
    public LayerMask kickMask;

    [Header("Polish")]
    public float cameraKickPunch = 4f;

    float nextKickTime;
    Camera cam;

    PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Kick.performed += _ => TryKick();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void Start()
    {
        cam = Camera.main;
    }

    void TryKick()
    {
        if (Time.time < nextKickTime)
            return;

        nextKickTime = Time.time + kickCooldown;
        DoKick();
    }

    void DoKick()
    {
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;

        if (Physics.SphereCast(
            origin,
            kickRadius,
            dir,
            out RaycastHit hit,
            kickRange,
            kickMask,
            QueryTriggerInteraction.Ignore))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;

            if (rb != null)
            {
                rb.AddForce(dir * kickForce, ForceMode.Impulse);
            }

            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.Die(dir * kickForce);
            }
        }

        KickPolish();
    }

    void KickPolish()
    {
        iTween.PunchRotation(
            cam.gameObject,
            new Vector3(-cameraKickPunch, 0f, 0f),
            0.12f
        );

        iTween.PunchPosition(
            cam.gameObject,
            new Vector3(0f, -0.05f, 0f),
            0.1f
        );
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + transform.forward * kickRange,
            kickRadius
        );
    }
#endif
}
