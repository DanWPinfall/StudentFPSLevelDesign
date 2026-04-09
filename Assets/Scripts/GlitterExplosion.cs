using UnityEngine;

public class GlitterExplosion : MonoBehaviour
{
    public float radius = 4f;
    public float damage = 999f;
    public float duration = 0.3f;
    public AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        audioSource.PlayOneShot(audioClip);
        iTween.ShakePosition(
            Camera.main.gameObject,
            new Vector3(0.4f, 0.4f, 0),
            0.25f
        );

        iTween.ShakeRotation(
            Camera.main.gameObject,
            new Vector3(6f, 6f, 2f),
            0.25f
        );

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                //Vector3 force = 2f;
                //enemy.Die(force);
            }
        }

        Destroy(gameObject, duration);
    }
}
