using UnityEngine;

public class BallMovement : MonoBehaviour
{
    // Ball ki starting speed
    [SerializeField] private float speed = 5f;

    // Har paddle collision par ball ki speed kitni increase hogi
    [SerializeField] private float speedIncrease = 0.3f;

    // Ball ko bilkul horizontal move karne se rokne ke liye
    // minimum vertical direction
    [SerializeField] private float minimumVerticalDirection = 0.25f;

    // Ball ki maximum allowed speed
    // Speed is value se zyada nahi hogi
    [SerializeField] private float maxSpeed = 10f;

    // Paddle ka Transform reference
    [SerializeField] private Transform paddle;

    // Ball ke Rigidbody2D ka reference
    private Rigidbody2D rb;

    // Ball ki starting launch direction
    private Vector2 startDirection;

    // Check karega ball currently move kar rahi hai ya nahi
    private bool canMove;

    // Paddle aur Ball ke darmiyan vertical gap
    [SerializeField] private float gapBetweenPaddleAndBall = 0.4f;

    private void Awake()
    {
        // Agar Paddle Inspector se assign nahi hai
        // to Paddle tag ke zariye object find karna
        if (paddle == null)
        {
            GameObject paddleObject =
                GameObject.FindGameObjectWithTag("Paddle");

            // Agar Paddle object mil gaya
            // to uska Transform reference save karna
            if (paddleObject != null)
            {
                paddle = paddleObject.transform;
            }
        }

        // Isi Ball GameObject ka Rigidbody2D lena
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Ball ki starting direction up-right banana
        //
        // X = 1 matlab right
        // Y = 1 matlab up
        //
        // normalized vector ki length 1 karta hai
        // lekin direction same rakhta hai
        startDirection =
            new Vector2(1f, 1f).normalized;
    }

    private void Update()
    {
        // Agar ball abhi launch nahi hui
        if (!canMove)
        {
            // Ball ko Paddle ke upar rakhna
            transform.position = new Vector2(
                paddle.position.x,
                paddle.position.y + gapBetweenPaddleAndBall
            );

            // Space key press hone par ball launch karna
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LaunchBall();
            }
        }
    }

    // Ball ko start direction aur current speed ke saath launch karna
    public void LaunchBall()
    {
        // Ball ko moving state mein lana
        canMove = true;

        // Direction × Speed = Velocity
        rb.linearVelocity =
            startDirection * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Agar Ball Paddle se collide kare
        if (collision.collider.CompareTag("Paddle"))
        {
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance.PlayPaddleHit();
            }

            // Paddle ki total width ko 2 se divide karke
            // paddle ki half width lena
            float halfWidth =
                collision.collider.bounds.size.x / 2f;

            // Check karna ball paddle ke center se
            // kitni left ya right side par lagi
            //
            // Left edge   ≈ -1
            // Center      ≈ 0
            // Right edge  ≈ 1
            float hitPosition =
                (
                    transform.position.x -
                    collision.transform.position.x
                )
                / halfWidth;

            // Hit position ko -1 aur 1 ke darmiyan rakhna
            // taake unexpected value na aaye
            hitPosition = Mathf.Clamp(
                hitPosition,
                -1f,
                1f
            );

            // Agar hit paddle ke bilkul center ke qareeb ho
            // to safe left ya right direction dena
            hitPosition =
                GetSafeHitPosition(hitPosition);

            // Hit position se nayi direction banana
            //
            // Negative X = up-left
            // Positive X = up-right
            // Y = 1      = upar
            Vector2 newDirection =
                new Vector2(hitPosition, 1f).normalized;

            // Har paddle collision par speed increase karna
            //speed += speedIncrease;

            // Speed ko maxSpeed se zyada jane se rokna
            //speed = Mathf.Min(
            //    speed,
            //    maxSpeed
            //);

            // Ball ko nayi direction aur updated speed dena
            rb.linearVelocity =
                newDirection * speed;
        }

        if (collision.collider.CompareTag("Boundaries"))
        {
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance.PlayBoundaryHit();
            }
        }

        // Har collision ke baad check karna
        // ke Ball almost horizontal to nahi chal rahi
        FixHorizontalMovement();
    }

    // Ball ko bilkul horizontal left-right loop mein
    // phasne se bachane wala method
    private void FixHorizontalMovement()
    {
        // Ball ki current velocity lena
        Vector2 currentVelocity =
            rb.linearVelocity;

        // Current actual speed save karna
        //
        // magnitude velocity vector ki total length hoti hai
        float currentSpeed =
            currentVelocity.magnitude;

        // Current velocity se sirf direction lena
        //
        // normalized ke baad direction ki length 1 ho jayegi
        Vector2 direction =
            currentVelocity.normalized;

        // Agar vertical Y direction bohat kam hai
        // to Ball almost horizontal move kar rahi hai
        if (Mathf.Abs(direction.y) <
            minimumVerticalDirection)
        {
            // Agar Ball upar ja rahi thi
            // to positive minimum Y dena
            //
            // Agar Ball neeche ja rahi thi
            // to negative minimum Y dena
            float newY =
                currentVelocity.y >= 0f
                    ? minimumVerticalDirection
                    : -minimumVerticalDirection;

            // Direction ki Y value fix karna
            direction.y = newY;

            // Y change karne ke baad vector ki length
            // dobara 1 karna
            direction = direction.normalized;

            // Corrected direction ke saath
            // same actual speed preserve karna
            rb.linearVelocity =
                direction * currentSpeed;
        }
    }

    // Paddle ke center ke qareeb hit ko
    // safe left ya right direction dena
    private float GetSafeHitPosition(
        float hitPosition
    )
    {
        // Mathf.Abs negative sign ko ignore karta hai
        //
        // Abs(-0.1) = 0.1
        // Abs(0.1)  = 0.1
        //
        // Dono values center ke qareeb hain
        if (Mathf.Abs(hitPosition) < 0.25f)
        {
            // Agar Ball center se thori left lagi
            // to minimum left direction dena
            if (hitPosition < 0f)
            {
                return -0.25f;
            }

            // Agar Ball center se thori right lagi
            // to minimum right direction dena
            else if (hitPosition > 0f)
            {
                return 0.25f;
            }

            // Agar Ball bilkul exact center par lagi
            else
            {
                // Agar Ball collision se pehle
                // left direction mein ja rahi thi
                if (rb.linearVelocity.x < 0f)
                {
                    return -0.25f;
                }

                // Warna Ball ko minimum right direction dena
                return 0.25f;
            }
        }

        // Agar hit center se door hai
        // to original hit position return karna
        return hitPosition;
    }

    // DeadZone mein jane ke baad
    // Ball ko Paddle ke paas reset karna
    public void ResetBall()
    {
        // Ball ki current movement rokna
        rb.linearVelocity = Vector2.zero;

        // Ball ko Paddle ke upar wapas lana
        transform.position = new Vector2(
            paddle.position.x,
            paddle.position.y + gapBetweenPaddleAndBall
        );

        // Ball ko dobara launch hone tak
        // Paddle ke saath rakhna
        canMove = false;
    }
}