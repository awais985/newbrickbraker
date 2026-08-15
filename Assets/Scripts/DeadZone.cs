using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        // Sirf Ball detect karna
        if (!other.CompareTag("Ball"))
        {
            return;
        }

        // BallMovement component lena
        BallMovement ballMovement =
            other.GetComponent<BallMovement>();

        if (ballMovement != null)
        {
            // Ab direct LoseLife nahi.

            // Pehle:
            // 💥 Explosion
            // phir delay
            // phir LoseLife
            ballMovement.ExplodeBall();
            GameManager.instance.LoseLife(ballMovement);
        }
    }
}