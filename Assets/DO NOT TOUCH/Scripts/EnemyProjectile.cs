using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 15f;
    public float life = 4f;

    void Start()
    {
        Destroy(gameObject, life);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
