using System.Collections;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    // =========================================================
    // SAFE ZONE
    // =========================================================

    [Header("Safe Zone")]
    [SerializeField] private GameObject safeZone;
    [SerializeField] private float safeZoneDuration = 5f;

    private Coroutine safeZoneCoroutine;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Start()
    {
        // Game start par SafeZone hidden
        if (safeZone != null)
        {
            safeZone.SetActive(false);
        }
    }


    // =========================================================
    // BOTTOM SHIELD POWER-UP
    // =========================================================

    public void BottomShield()
    {
        // Agar pehle se timer chal raha hai
        // to purana timer stop karke fresh 5 sec dena
        if (safeZoneCoroutine != null)
        {
            StopCoroutine(safeZoneCoroutine);
        }

        safeZoneCoroutine =
            StartCoroutine(SafeZoneShow());
    }


    private IEnumerator SafeZoneShow()
    {
        if (safeZone == null)
        {
            yield break;
        }

        // Shield show
        safeZone.SetActive(true);

        // 5 seconds active
        yield return new WaitForSeconds(
            safeZoneDuration
        );

        // Shield hide
        safeZone.SetActive(false);

        safeZoneCoroutine = null;
    }


    // =========================================================
    // TRIGGER DETECTION
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        // =====================================================
        // BALL
        // =====================================================

        if (other.CompareTag("Ball"))
        {
            // Lose-life sound
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance.PlayBallFall();
            }

            BallMovement ballMovement =
                other.GetComponent<BallMovement>();

            if (ballMovement == null)
            {
                return;
            }

            if (GameManager.instance != null)
            {
                GameManager.instance.UnregisterBall(
                    ballMovement
                );
            }

            return;
        }


        // =====================================================
        // POWER-UP
        // =====================================================

        if (other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
        }
    }
}