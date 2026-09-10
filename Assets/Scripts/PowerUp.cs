using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power Up Settings")]
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private float fallSpeed = 2.5f;

    [Header("Pickup Effect")]
    [SerializeField] private float pickupScale = 1.25f;
    [SerializeField] private float pickupDuration = 0.12f;

    private bool isCollected;

    private void Update()
    {
        if (isCollected) return;
        RunPowerUp();
    }

    public void RunPowerUp()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Paddle") || isCollected) return;

        isCollected = true;
        ApplyPowerUp(other);
        StartCoroutine(PickupEffect());
    }

    private void ApplyPowerUp(Collider2D paddleCollider)
    {
        switch (powerUpType)
        {
            case PowerUpType.ExtraLife:
                if (GameManager.instance != null)
                    GameManager.instance.AddLife();
                break;
            case PowerUpType.BottomShield:
                if(GameManager.instance != null)
                {
                    GameManager.instance.BottomShield();
                }
                break;

            case PowerUpType.ExpandPaddle:
                PaddleController paddle = paddleCollider.GetComponent<PaddleController>();
                if (paddle != null)
                    paddle.ExpandPaddle();
                break;

            case PowerUpType.ShrinkPaddle:
                PaddleController paddle1 = paddleCollider.GetComponent<PaddleController>();

                if (paddle1 != null)
                {
                    paddle1.ShinkPaddle();

                }
                break;

            // MASLA 2 FIX: Ab screen par majood Tamam Balls slow ho jayengi
            case PowerUpType.SlowBall:
                BallMovement[] activeBalls = FindObjectsByType<BallMovement>(FindObjectsSortMode.None);
                foreach (BallMovement ball in activeBalls)
                {
                    ball.SlowBall();
                }
                break;

            case PowerUpType.MultiBall:
                if (GameManager.instance != null && GameManager.instance.CanSpawnMultiBall())
                {
                    BallMovement[] currentBalls = FindObjectsByType<BallMovement>(FindObjectsSortMode.None);
                    foreach (BallMovement ball in currentBalls)
                    {
                        ball.CreateMultiBall();
                    }
                }
                break;
        }
    }

    private IEnumerator PickupEffect()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * pickupScale;

        while (timer < pickupDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / pickupDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}