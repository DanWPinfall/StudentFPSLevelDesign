using UnityEngine;

public class StickerProjectile : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 10f;

    public int damage = 1;


    Rigidbody rb;
    bool stuck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.angularVelocity = Random.insideUnitSphere * 8f;

        rb.linearVelocity = transform.forward * speed;
        //Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (stuck) return;
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 12) return;

        stuck = true;

        ContactPoint contact = collision.contacts[0];

        transform.position = contact.point;
        transform.rotation = Quaternion.LookRotation(-contact.normal);

        Rigidbody hitRB = collision.rigidbody;

        if (hitRB != null)
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = hitRB;

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = hitRB.transform.InverseTransformPoint(contact.point);

            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;

            rb.mass = 0.02f;
            rb.linearDamping = 5f;
            rb.angularDamping = 5f;
        }

        else
        {

            rb.isKinematic = true;
            transform.SetParent(collision.transform, true);
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (collision.collider.TryGetComponent(out Enemy enemy))
        {
            Vector3 force = rb.linearVelocity * 2f; 
            enemy.Die(force);
        }
    }
}
