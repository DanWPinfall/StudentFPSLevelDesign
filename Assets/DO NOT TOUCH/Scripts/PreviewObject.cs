using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 30f;

    [Header("Pulse")]
    public float pulseAmount = 0.1f;
    public float pulseTime = 1f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", originalScale + Vector3.one * pulseAmount,
            "time", pulseTime,
            "easetype", iTween.EaseType.easeInOutSine,
            "looptype", iTween.LoopType.pingPong
        ));
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}