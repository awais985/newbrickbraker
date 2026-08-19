using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // =========================================================
    // POWER-UP SETTINGS
    // =========================================================

    [Header("Power Up Settings")]

    // Ye decide karta hai ke ye prefab
    // kis type ka PowerUp hai
    [SerializeField] private PowerUpType powerUpType;

    // PowerUp kitni speed se neeche girega
    [SerializeField] private float fallSpeed = 2.5f;


    // =========================================================
    // PICKUP EFFECT SETTINGS
    // =========================================================

    [Header("Pickup Effect")]

    // Pickup par PowerUp kitna bada hoga
    [SerializeField] private float pickupScale = 1.25f;

    // Pickup animation kitni der chalegi
    [SerializeField] private float pickupDuration = 0.12f;


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // Check karega ke PowerUp already collect
    // ho chuka hai ya nahi
    private bool isCollected;


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void Update()
    {
        // Agar PowerUp already collect ho gaya hai
        // to usko neeche move nahi karna
        if (isCollected)
        {
            return;
        }

        // PowerUp ko world-space mein
        // seedha neeche move karna

        RunPowerUp();


    }

    public void RunPowerUp()
    {
        transform.Translate(
           Vector3.down * fallSpeed * Time.deltaTime,
           Space.World
       );
    }

    // =========================================================
    // PADDLE PICKUP
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sirf Paddle PowerUp collect karega
        if (!other.CompareTag("Paddle"))
        {
            return;
        }

        // Agar PowerUp pehle hi collect ho chuka hai
        // to dobara effect apply nahi karna
        if (isCollected)
        {
            return;
        }

        isCollected = true;


        // =====================================================
        // PICKUP SOUND
        // =====================================================

        if (AudioClipManager.instance != null)
        {
            // AudioClipManager.instance.PlayPowerUpPickup();
        }


        // =====================================================
        // APPLY POWER-UP EFFECT
        // =====================================================

        ApplyPowerUp(other);


        // =====================================================
        // PICKUP VISUAL EFFECT
        // =====================================================

        StartCoroutine(PickupEffect());
    }


    // =========================================================
    // POWER-UP TYPE HANDLING
    // =========================================================

    private void ApplyPowerUp(Collider2D paddleCollider)
    {
        switch (powerUpType)
        {
            // -------------------------------------------------
            // EXTRA LIFE
            // -------------------------------------------------

            case PowerUpType.ExtraLife:

                if (GameManager.instance != null)
                {
                    GameManager.instance.AddLife();
                }

                break;


            // -------------------------------------------------
            // EXPAND PADDLE
            // -------------------------------------------------

            case PowerUpType.ExpandPaddle:

                PaddleController paddle =
                    paddleCollider.GetComponent<PaddleController>();

                if (paddle != null)
                {
                    paddle.ExpandPaddle();
                }

                break;


            // -------------------------------------------------
            // SLOW BALL
            // -------------------------------------------------

            case PowerUpType.SlowBall:

                BallMovement slowBall =
                    FindFirstObjectByType<BallMovement>();

                if (slowBall != null)
                {
                    slowBall.SlowBall();
                }

                break;


            // -------------------------------------------------
            // MULTI BALL
            // -------------------------------------------------

            case PowerUpType.MultiBall:

                // Pehle check karna ke
                // MultiBall currently allowed hai ya nahi
                if (GameManager.instance != null &&
                    GameManager.instance.CanSpawnMultiBall())
                {
                    BallMovement multiBall =
                        FindFirstObjectByType<BallMovement>();

                    if (multiBall != null)
                    {
                        multiBall.CreateMultiBall();
                    }
                }

                break;
        }
    }


    // =========================================================
    // PICKUP SCALE EFFECT
    // =========================================================

    private IEnumerator PickupEffect()
    {
        float timer = 0f;

        // PowerUp ki current scale save karna
        Vector3 startScale =
            transform.localScale;

        // Pickup par thora bada target size
        Vector3 targetScale =
            startScale * pickupScale;


        // Short pop animation
        while (timer < pickupDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                timer / pickupDuration;

            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    targetScale,
                    progress
                );

            yield return null;
        }


        // Animation complete hone ke baad
        // PowerUp remove karna
        Destroy(gameObject);
    }
}