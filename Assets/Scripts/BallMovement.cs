using System.Collections;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    // Ball ki starting speed
    [SerializeField] private float speed = 5f;

    // Har paddle collision par ball ki speed kitni increase hogi
    [SerializeField] private float speedIncrease = 0.3f;

    // Ball ko bilkul horizontal move karne se rokne ke liye
    [SerializeField] private float minimumVerticalDirection = 0.25f;

    // Ball ki maximum allowed speed
    [SerializeField] private float maxSpeed = 10f;

    // Paddle ka Transform reference
    [SerializeField] private Transform paddle;

    [Header("Trail")]
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Death Explosion")]
    [SerializeField] private ParticleSystem deathExplosionPrefab;

    // Explosion ke baad reset hone se pehle delay
    [SerializeField] private float deathDelay = 0.35f;

    // Ball ke Rigidbody2D ka reference
    private Rigidbody2D rb;

    // Ball SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Ball Collider
    private Collider2D ballCollider;

    // Ball ki starting launch direction
    private Vector2 startDirection;

    // Check karega ball currently move kar rahi hai ya nahi
    private bool canMove;

    // Check karega ball death animation mein hai ya nahi
    private bool isDying;

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

            if (paddleObject != null)
            {
                paddle = paddleObject.transform;
            }
        }

        // Trail Renderer automatically lena
        if (trailRenderer == null)
        {
            trailRenderer =
                GetComponent<TrailRenderer>();
        }

        // Rigidbody2D lena
        rb = GetComponent<Rigidbody2D>();

        // Sprite Renderer lena
        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();

        // Ball Collider lena
        ballCollider =
            GetComponent<Collider2D>();
    }

    private void Start()
    {
        // Starting state mein trail band
        SetTrailActive(false);

        // Ball ki starting direction up-right banana
        startDirection =
            new Vector2(1f, 1f).normalized;
    }

    private void Update()
    {
        // Agar ball death animation mein hai
        // to paddle follow / launch mat karo
        if (isDying)
        {
            return;
        }

        // Agar ball abhi launch nahi hui
        if (!canMove)
        {
            // Ball ko Paddle ke upar rakhna
            transform.position = new Vector2(
                paddle.position.x,
                paddle.position.y +
                gapBetweenPaddleAndBall
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
        // Agar already move kar rahi hai ya dying hai
        // to dobara launch nahi karna
        if (canMove || isDying)
        {
            return;
        }

        canMove = true;

        // Purani trail clear karke
        // launch ke waqt trail start karna
        SetTrailActive(true);

        // Direction × Speed = Velocity
        rb.linearVelocity =
            startDirection * speed;
    }

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        // Death ke waqt collision logic nahi chalana
        if (isDying)
        {
            return;
        }

        // Agar Ball Paddle se collide kare
        if (collision.collider.CompareTag("Paddle"))
        {
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance
                    .PlayPaddleHit();
            }

            // Paddle ki half width
            float halfWidth =
                collision.collider.bounds.size.x / 2f;

            // Ball paddle ke center se kitni
            // left/right lagi
            float hitPosition =
                (
                    transform.position.x -
                    collision.transform.position.x
                )
                / halfWidth;

            // -1 aur 1 ke darmiyan rakhna
            hitPosition = Mathf.Clamp(
                hitPosition,
                -1f,
                1f
            );

            // Center ke qareeb hit fix karna
            hitPosition =
                GetSafeHitPosition(hitPosition);

            // Nayi upward direction
            Vector2 newDirection =
                new Vector2(
                    hitPosition,
                    1f
                ).normalized;

            /*
            // Agar baad mein speed increase enable karni ho:

            speed += speedIncrease;

            speed = Mathf.Min(
                speed,
                maxSpeed
            );
            */

            rb.linearVelocity =
                newDirection * speed;
        }

        // Boundaries hit sound
        if (collision.collider.CompareTag("Boundaries"))
        {
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance
                    .PlayBoundaryHit();
            }
        }

        // Horizontal movement fix
        FixHorizontalMovement();
    }

    private void FixHorizontalMovement()
    {
        Vector2 currentVelocity =
            rb.linearVelocity;

        float currentSpeed =
            currentVelocity.magnitude;

        // Agar ball move hi nahi kar rahi
        // to kuch fix nahi karna
        if (currentSpeed <= 0f)
        {
            return;
        }

        Vector2 direction =
            currentVelocity.normalized;

        if (Mathf.Abs(direction.y) <
            minimumVerticalDirection)
        {
            float newY =
                currentVelocity.y >= 0f
                    ? minimumVerticalDirection
                    : -minimumVerticalDirection;

            direction.y = newY;

            direction =
                direction.normalized;

            rb.linearVelocity =
                direction * currentSpeed;
        }
    }

    private void SetTrailActive(
        bool active
    )
    {
        if (trailRenderer == null)
        {
            return;
        }

        // Pehle emitting state change
        trailRenderer.emitting = active;

        // Purani positions remove
        trailRenderer.Clear();
    }

    private float GetSafeHitPosition(
        float hitPosition
    )
    {
        if (Mathf.Abs(hitPosition) < 0.25f)
        {
            if (hitPosition < 0f)
            {
                return -0.25f;
            }
            else if (hitPosition > 0f)
            {
                return 0.25f;
            }
            else
            {
                if (rb.linearVelocity.x < 0f)
                {
                    return -0.25f;
                }

                return 0.25f;
            }
        }

        return hitPosition;
    }

    // DeadZone is method ko call karega
    public void ExplodeBall()
    {
        // Ek hi death par multiple explosion
        // hone se rokna
        if (isDying)
        {
            return;
        }

        StartCoroutine(
            DeathRoutine()
        );
    }

    private IEnumerator DeathRoutine()
    {
        // Ball ko death state mein lana
        isDying = true;

        // Normal movement band
        canMove = false;

        // Ball stop
        rb.linearVelocity =
            Vector2.zero;

        rb.angularVelocity = 0f;

        // Trail turant band + clear
        SetTrailActive(false);

        // Explosion create karna
        if (deathExplosionPrefab != null)
        {
            ParticleSystem explosion =
                Instantiate(
                    deathExplosionPrefab,
                    transform.position,
                    Quaternion.identity
                );

            explosion.Play();

            // Explosion object ko baad mein remove
            Destroy(
                explosion.gameObject,
                2f
            );
        }

        // Ball miss hone par halka camera shake
        if (CameraShake.instance != null)
        {
            CameraShake.instance.Shake();
        }

        // Ball ko visually hide
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // DeadZone dobara trigger na kare
        if (ballCollider != null)
        {
            ballCollider.enabled = false;
        }

        // Explosion ko visible hone ka time
        yield return new WaitForSeconds(
            deathDelay
        );

        // Ab life lose karna
        if (GameManager.instance != null)
        {
            GameManager.instance
                .LoseLife(this);
        }
        else
        {
            // Safety fallback
            ResetBall();
        }
    }

    // DeadZone mein jane ke baad
    // Ball ko Paddle ke paas reset karna
    public void ResetBall()
    {
        // Movement stop
        rb.linearVelocity =
            Vector2.zero;

        rb.angularVelocity = 0f;

        // Trail reset se PEHLE band
        SetTrailActive(false);

        // Ball ko paddle ke upar lana
        transform.position = new Vector2(
            paddle.position.x,
            paddle.position.y +
            gapBetweenPaddleAndBall
        );

        // Ball Sprite wapas show
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // Collider wapas enable
        if (ballCollider != null)
        {
            ballCollider.enabled = true;
        }

        // Death complete
        isDying = false;

        // Ball paddle ke saath wait karegi
        canMove = false;

        // Extra safety clear
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
    }
}