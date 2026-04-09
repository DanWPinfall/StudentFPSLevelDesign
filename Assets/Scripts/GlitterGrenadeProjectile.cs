using UnityEngine;

public class GlitterGrenadeProjectile : MonoBehaviour
{
    public float speed = 25f;
    public float fuseTime = 0.2f;
    public GameObject explosionPrefab;

    Rigidbody rb;
    bool stuck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        rb.angularVelocity = Random.insideUnitSphere * 6f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (stuck) return;
        stuck = true;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        ContactPoint contact = collision.contacts[0];
        transform.position = contact.point;
        transform.rotation = Quaternion.LookRotation(contact.normal);

        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        Time.timeScale = 0.85f;
        Invoke(nameof(ResetTime), 0.05f);

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void ResetTime()
    {
        Time.timeScale = 1f;
    }

}
