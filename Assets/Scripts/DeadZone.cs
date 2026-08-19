using UnityEngine;

public class DeadZone : MonoBehaviour
{
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
            BallMovement ballMovement =
                other.GetComponent<BallMovement>();

            if (ballMovement == null)
            {
                return;
            }

            // GameManager ko batana ke ye ball miss ho gayi
            //
            // IMPORTANT:
            // Ball death / life loss ka main decision
            // ek hi jagah handle karna better hai.
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
            // Jo PowerUp paddle collect nahi kar paya
            // wo bottom DeadZone touch karte hi remove ho jayega
            Destroy(other.gameObject);
        }
    }
}