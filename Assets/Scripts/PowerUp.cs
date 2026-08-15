using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private float fallSpeed = 2.5f;

    private void Update()
    {
        // Power-up ko neeche move karna
        transform.Translate(
            Vector3.down * fallSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sirf Paddle power-up collect karega
        if (!other.CompareTag("Paddle"))
        {
            return;
        }

        Debug.Log("Collected PowerUp: " + powerUpType);

        // Effect baad mein yahan activate karenge

        Destroy(gameObject);
    }
}