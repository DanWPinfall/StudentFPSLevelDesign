using UnityEngine;

public static class NoiseSystem
{
    public static void EmitNoise(Vector3 position, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.OnHearNoise(position);
            }
        }
    }
}
