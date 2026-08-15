using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakeStrength = 0.07f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        instance = this;

        // Camera ki original local position save karna
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        // Agar pehle se shake chal raha ho
        // to usko stop karke fresh shake start karna
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(
            ShakeRoutine()
        );
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            // Random 2D direction lena
            Vector2 randomOffset = Random.insideUnitCircle * shakeStrength;

            // Camera ko original position ke around
            // halka sa move karna
            transform.localPosition =
                originalPosition +
                new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            timer += Time.unscaledDeltaTime;

            yield return null;
        }

        // Shake complete hone ke baad
        // camera ko exact original position par lana
        transform.localPosition =
            originalPosition;

        shakeCoroutine = null;
    }
}