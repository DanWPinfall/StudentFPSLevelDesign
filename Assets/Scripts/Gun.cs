using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public float fireRate = 0.15f;
    public float range = 100f;
    public LayerMask hitMask;

    float nextFireTime;
    Camera cam;

    public GameObject stickerPrefab;
    public Transform firePoint;
    public float noiseRadius = 12f;
    AudioSource audioSource;
    public AudioClip shotClip;
    public AudioClip glitterGrenadeClip;

    [Header("Recoil")]
    public float kickBackDistance = 0.18f;
    public float kickBackTime = 0.06f;
    public float returnTime = 0.08f;

    Vector3 gunStartLocalPos;
    Vector3 gunStartLocalRot;

    [Header("Movement Sway")]
    public float swayAmount = 0.05f;
    public float swaySmooth = 10f;
    public float bobAmount = 0.025f;
    public float bobSpeed = 12f;
    Vector3 swayOffset;

    [Header("Glitter Grenade")]
    public GameObject glitterGrenadePrefab;
    public Transform underbarrelFirePoint;
    public bool hasGlitterGrenade = true;



    PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Fire.performed += _ => TryFire();
        input.Player.AltFire.performed += _ => FireGlitterGrenade();

    }

    void OnDisable()
    {
        input.Player.Disable();
        input.Player.AltFire.performed -= _ => FireGlitterGrenade();

    }

    void Start()
    {
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>();

        gunStartLocalPos = transform.localPosition;
        gunStartLocalRot = transform.localEulerAngles;
    }

    void Update()
    {
        HandleMovementSway();
    }

    void TryFire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        Fire();
    }
    void Fire()
    {
        KickGun();

        //iTween.PunchRotation(cam.gameObject,
        //    new Vector3(-6f, Random.Range(-1f, 1f), 0),
        //    0.12f);

        //iTween.ShakePosition(cam.gameObject,
        //    new Vector3(0.05f, 0.05f, 0),
        //    0.1f);

        iTween.PunchPosition(
             cam.gameObject,
             new Vector3(0f, -0.07f, 0),
             0.06f
         );

        iTween.PunchRotation(
            cam.gameObject,
            new Vector3(-3f, Random.Range(-0.5f, 0.5f), 0),
            0.07f
        );



        Instantiate(
            stickerPrefab,
            firePoint.position,
            firePoint.rotation
        );

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(shotClip);

        NoiseSystem.EmitNoise(transform.position, noiseRadius);

    }

    void KickGun()
    {
        iTween.Stop(gameObject);

        iTween.MoveTo(
            gameObject,
            iTween.Hash(
                "islocal", true,
                "position", gunStartLocalPos - Vector3.forward * kickBackDistance,
                "time", kickBackTime,
                "easetype", iTween.EaseType.easeOutQuad,
                "oncomplete", "ReturnGun"
            )
        );
    }

    void ReturnGun()
    {
        iTween.MoveTo(
            gameObject,
            iTween.Hash(
                "islocal", true,
                "position", gunStartLocalPos,
                "time", returnTime,
                "easetype", iTween.EaseType.easeOutExpo
            )
        );
    }

    void HandleMovementSway()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();

        Vector3 targetSway =
            transform.right * move.x * swayAmount +
            transform.up * Mathf.Abs(move.y) * -swayAmount * 0.5f;

        if (move.magnitude > 0.1f)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            targetSway += transform.up * bob;
        }

        swayOffset = Vector3.Lerp(
            swayOffset,
            targetSway,
            Time.deltaTime * swaySmooth
        );

        transform.localPosition = gunStartLocalPos + swayOffset;

        float rotZ = -move.x * 1.5f;
        transform.localRotation = Quaternion.Euler(
            gunStartLocalRot.x,
            gunStartLocalRot.y,
            rotZ
        );

    }

    void FireGlitterGrenade()
    {
        if (!hasGlitterGrenade) return;

        hasGlitterGrenade = false;

        audioSource.PlayOneShot(glitterGrenadeClip);

        Instantiate(
            glitterGrenadePrefab,
            underbarrelFirePoint.position,
            underbarrelFirePoint.rotation
        );

        iTween.PunchRotation(
            cam.gameObject,
            new Vector3(-6f, 0f, 0f),
            0.15f
        );
    }

}
