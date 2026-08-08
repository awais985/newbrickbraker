using UnityEngine;

public class DeadZone : MonoBehaviour
{
    // Jab koi Collider2D DeadZone ke trigger area mein enter kare
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sirf us object ko check karna
        // jiska tag "Ball" hai
        if (other.CompareTag("Ball"))
        {
            // DeadZone mein enter hone wali Ball se
            // BallMovement component lena
            BallMovement ballMovement =
                other.GetComponent<BallMovement>();

            // Agar BallMovement component mil gaya
            if (ballMovement != null)
            {
                // Ball ki movement rok kar
                // usay Paddle ke upar wapas reset karna
                GameManager.instance.LoseLife(ballMovement);
            }
        }
    }
}