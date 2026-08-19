using System.Collections;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    // =========================================================
    // MOVEMENT SETTINGS
    // =========================================================

    [Header("Movement Settings")]

    // Ball ki normal starting speed
    [SerializeField] private float speed = 5f;

    // Future mein har paddle hit par
    // kitni speed increase karni hai
    [SerializeField] private float speedIncrease = 0.3f;

    // Ball ko almost horizontal movement se bachana
    [SerializeField] private float minimumVerticalDirection = 0.25f;

    // Maximum allowed speed
    [SerializeField] private float maxSpeed = 10f;

    // Paddle reference
    [SerializeField] private Transform paddle;

    // Paddle aur waiting Ball ke darmiyan gap
    [SerializeField] private float gapBetweenPaddleAndBall = 0.4f;


    // =========================================================
    // TRAIL
    // =========================================================

    [Header("Trail")]

    [SerializeField] private TrailRenderer trailRenderer;


    // =========================================================
    // DEATH EFFECT
    // =========================================================

    [Header("Death Explosion")]

    // Ball miss hone par explosion prefab
    [SerializeField] private ParticleSystem deathExplosionPrefab;

    // Explosion ke baad ball unregister hone ka delay
    [SerializeField] private float deathDelay = 0.35f;


    // =========================================================
    // SLOW BALL POWER-UP
    // =========================================================

    [Header("Slow Ball Power Up")]

    // Normal speed ka kitna percent slow speed hogi
    [SerializeField] private float slowMultiplier = 0.65f;

    // Slow effect kitni der chalega
    [SerializeField] private float slowDuration = 5f;


    // =========================================================
    // RUNTIME REFERENCES
    // =========================================================

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D ballCollider;


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // Starting launch direction
    private Vector2 startDirection;

    // Ball launch hui hai ya Paddle ke saath wait kar rahi hai
    private bool canMove;

    // Ball currently death animation mein hai
    private bool isDying;

    // Check karega ye original Ball hai
    // ya MultiBall se create hui extra Ball
    private bool isExtraBall;

    // Normal speed save karna
    private float originalSpeed;

    // SlowBall coroutine
    private Coroutine slowCoroutine;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Rigidbody2D lena
        rb = GetComponent<Rigidbody2D>();


        // Ball sprite lena
        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();


        // Ball collider lena
        ballCollider =
            GetComponent<Collider2D>();


        // Agar Trail Inspector se assign nahi hai
        if (trailRenderer == null)
        {
            trailRenderer =
                GetComponent<TrailRenderer>();
        }


        // Agar Paddle Inspector se assign nahi hai
        // to Tag se automatically find karna
        if (paddle == null)
        {
            GameObject paddleObject =
                GameObject.FindGameObjectWithTag(
                    "Paddle"
                );

            if (paddleObject != null)
            {
                paddle = paddleObject.transform;
            }
        }
    }


    private void Start()
    {
        // Ball ki normal speed save karna
        originalSpeed = speed;


        // Default launch direction
        startDirection =
            new Vector2(1f, 1f).normalized;


        // Starting state mein trail off
        SetTrailActive(false);
    }


    // =========================================================
    // UPDATE / WAITING ON PADDLE
    // =========================================================

    private void Update()
    {
        // Death animation ke waqt
        // normal Ball behaviour nahi chalana
        if (isDying)
        {
            return;
        }


        // Agar Ball already launch ho chuki hai
        if (canMove)
        {
            return;
        }


        // Safety check
        if (paddle == null)
        {
            return;
        }


        // Ball ko Paddle ke upar follow karwana
        transform.position =
            new Vector2(
                paddle.position.x,
                paddle.position.y +
                gapBetweenPaddleAndBall
            );


        // Space press par Ball launch
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall();
        }
    }


    // =========================================================
    // LAUNCH BALL
    // =========================================================

    public void LaunchBall()
    {
        // Already launch hui hai
        // ya death animation mein hai
        if (canMove || isDying)
        {
            return;
        }


        if (rb == null)
        {
            return;
        }


        canMove = true;


        // Fresh trail start
        SetTrailActive(true);


        // Direction × Speed
        rb.linearVelocity =
            startDirection * speed;
    }


    // =========================================================
    // EXTRA BALL DATA
    // =========================================================

    public void SetAsExtraBall()
    {
        isExtraBall = true;
    }


    public bool IsExtraBall()
    {
        return isExtraBall;
    }


    // =========================================================
    // VELOCITY HELPERS
    // =========================================================

    public Vector2 GetCurrentVelocity()
    {
        if (rb == null)
        {
            return Vector2.zero;
        }

        return rb.linearVelocity;
    }


    public void SetVelocity(Vector2 velocity)
    {
        if (rb == null)
        {
            return;
        }


        // Extra Ball directly moving state mein jayegi
        canMove = true;

        rb.linearVelocity =
            velocity;


        SetTrailActive(true);
    }


    // =========================================================
    // SLOW BALL POWER-UP
    // =========================================================

    public void SlowBall()
    {
        // Agar SlowBall already active hai,
        // previous timer cancel karke fresh timer start karna
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }


        slowCoroutine =
            StartCoroutine(
                SlowBallRoutine()
            );
    }


    private IEnumerator SlowBallRoutine()
    {
        // Speed slow karna
        speed =
            originalSpeed *
            slowMultiplier;


        // Agar Ball currently move kar rahi hai
        // to current direction preserve karna
        if (rb != null &&
            rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                speed;
        }


        // Temporary power-up duration
        yield return new WaitForSeconds(
            slowDuration
        );


        // Normal speed restore
        speed = originalSpeed;


        // Current direction preserve karke
        // normal speed restore karna
        if (rb != null &&
            rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                speed;
        }


        slowCoroutine = null;
    }


    // =========================================================
    // MULTI BALL POWER-UP
    // =========================================================

    public void CreateMultiBall()
    {
        if (rb == null)
        {
            return;
        }


        Vector2 currentVelocity =
            rb.linearVelocity;


        // Standing Ball se MultiBall create nahi karna
        if (currentVelocity.sqrMagnitude < 0.01f)
        {
            return;
        }


        // -----------------------------------------------------
        // LEFT EXTRA BALL
        // -----------------------------------------------------

        BallMovement leftBall =
            Instantiate(
                this,
                transform.position,
                Quaternion.identity
            );


        // -----------------------------------------------------
        // RIGHT EXTRA BALL
        // -----------------------------------------------------

        BallMovement rightBall =
            Instantiate(
                this,
                transform.position,
                Quaternion.identity
            );


        // Current Ball direction ko 25 degree
        // left aur right rotate karna
        Vector2 leftDirection =
            Quaternion.Euler(
                0f,
                0f,
                25f
            ) *
            currentVelocity.normalized;


        Vector2 rightDirection =
            Quaternion.Euler(
                0f,
                0f,
                -25f
            ) *
            currentVelocity.normalized;


        // Dono cloned balls ko
        // Extra Ball mark karna
        leftBall.SetAsExtraBall();
        rightBall.SetAsExtraBall();


        // Current magnitude preserve karna
        leftBall.SetVelocity(
            leftDirection *
            currentVelocity.magnitude
        );


        rightBall.SetVelocity(
            rightDirection *
            currentVelocity.magnitude
        );


        // GameManager ko batana:
        // do extra balls game mein add hui hain
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterBall();
            GameManager.instance.RegisterBall();
        }
    }


    // =========================================================
    // COLLISION
    // =========================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        // Death ke waqt collision ignore
        if (isDying)
        {
            return;
        }


        // =====================================================
        // PADDLE COLLISION
        // =====================================================

        if (collision.collider.CompareTag("Paddle"))
        {
            // Paddle hit sound
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance
                    .PlayPaddleHit();
            }


            // Paddle ki half width
            float halfWidth =
                collision.collider.bounds.size.x /
                2f;


            // Ball paddle ke center se
            // kitni left/right hit hui
            float hitPosition =
                (
                    transform.position.x -
                    collision.transform.position.x
                )
                /
                halfWidth;


            // Value ko -1 se +1 ke andar rakhna
            hitPosition =
                Mathf.Clamp(
                    hitPosition,
                    -1f,
                    1f
                );


            // Bilkul center hit ko
            // dangerous vertical direction se bachana
            hitPosition =
                GetSafeHitPosition(
                    hitPosition
                );


            // Ball hamesha Paddle se
            // upward bounce karegi
            Vector2 newDirection =
                new Vector2(
                    hitPosition,
                    1f
                ).normalized;


            /*
            // Future speed increase:

            speed += speedIncrease;

            speed = Mathf.Min(
                speed,
                maxSpeed
            );
            */


            rb.linearVelocity =
                newDirection *
                speed;
        }


        // =====================================================
        // BOUNDARY COLLISION
        // =====================================================

        if (collision.collider.CompareTag("Boundaries"))
        {
            Debug.Log(AudioClipManager.instance);
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance
                    .PlayBoundaryHit();
            }
        }


        // Collision ke baad
        // almost-horizontal movement fix karna
        FixHorizontalMovement();
    }


    // =========================================================
    // PREVENT HORIZONTAL BALL LOOP
    // =========================================================

    private void FixHorizontalMovement()
    {
        if (rb == null)
        {
            return;
        }


        Vector2 currentVelocity =
            rb.linearVelocity;


        float currentSpeed =
            currentVelocity.magnitude;


        // Ball stationary hai
        if (currentSpeed <= 0f)
        {
            return;
        }


        Vector2 direction =
            currentVelocity.normalized;


        // Agar vertical direction bohat kam hai
        if (Mathf.Abs(direction.y) <
            minimumVerticalDirection)
        {
            // Current up/down direction preserve karna
            float newY =
                currentVelocity.y >= 0f
                    ? minimumVerticalDirection
                    : -minimumVerticalDirection;


            direction.y = newY;

            direction =
                direction.normalized;


            // Current total speed preserve karna
            rb.linearVelocity =
                direction *
                currentSpeed;
        }
    }


    // =========================================================
    // SAFE PADDLE HIT
    // =========================================================

    private float GetSafeHitPosition(
        float hitPosition
    )
    {
        // Agar hit paddle ke bilkul center ke qareeb hai
        if (Mathf.Abs(hitPosition) < 0.25f)
        {
            if (hitPosition < 0f)
            {
                return -0.25f;
            }


            if (hitPosition > 0f)
            {
                return 0.25f;
            }


            // Exact center hit:
            // previous horizontal direction preserve karna
            if (rb != null &&
                rb.linearVelocity.x < 0f)
            {
                return -0.25f;
            }


            return 0.25f;
        }


        return hitPosition;
    }


    // =========================================================
    // TRAIL
    // =========================================================

    private void SetTrailActive(
        bool active
    )
    {
        if (trailRenderer == null)
        {
            return;
        }


        // Trail on / off
        trailRenderer.emitting =
            active;


        // Purani trail positions clear
        trailRenderer.Clear();
    }


    // =========================================================
    // BALL DEATH
    // =========================================================

    // DeadZone sirf is method ko call karega
    public void ExplodeBall()
    {
        // Same Ball ki death
        // multiple baar start nahi hone dena
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
        // Ball death state
        isDying = true;


        // Normal movement disable
        canMove = false;


        // Ball stop
        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity =
                0f;
        }


        // Trail stop
        SetTrailActive(false);


        // -----------------------------------------------------
        // EXPLOSION
        // -----------------------------------------------------

        if (deathExplosionPrefab != null)
        {
            ParticleSystem explosion =
                Instantiate(
                    deathExplosionPrefab,
                    transform.position,
                    Quaternion.identity
                );


            explosion.Play();


            // Temporary explosion object remove
            Destroy(
                explosion.gameObject,
                2f
            );
        }


        // -----------------------------------------------------
        // CAMERA SHAKE
        // -----------------------------------------------------

        if (CameraShake.instance != null)
        {
            CameraShake.instance.Shake();
        }


        // -----------------------------------------------------
        // HIDE BALL
        // -----------------------------------------------------

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled =
                false;
        }


        // DeadZone ko same Ball
        // dobara detect karne se rokna
        if (ballCollider != null)
        {
            ballCollider.enabled =
                false;
        }


        // Explosion visible hone ka time
        yield return new WaitForSeconds(
            deathDelay
        );


        // =====================================================
        // IMPORTANT:
        // Ball khud LoseLife nahi karegi.
        //
        // GameManager decide karega:
        // - extra ball?
        // - original ball?
        // - koi aur active ball baqi hai?
        // - last ball hai?
        // =====================================================

        if (GameManager.instance != null)
        {
            GameManager.instance
                .UnregisterBall(this);
        }
        else
        {
            ResetBall();
        }
    }


    // =========================================================
    // RESET BALL
    // =========================================================

    public void ResetBall()
    {
        // Agar SlowBall coroutine chal rahi ho
        // to usko stop karna
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }


        // Normal speed restore
        speed = originalSpeed;


        // Ball physics stop
        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity =
                0f;
        }


        // Trail off + clear
        SetTrailActive(false);


        // Paddle available ho to
        // Ball ko starting position par lana
        if (paddle != null)
        {
            transform.position =
                new Vector2(
                    paddle.position.x,
                    paddle.position.y +
                    gapBetweenPaddleAndBall
                );
        }


        // Sprite show
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled =
                true;
        }


        // Collider enable
        if (ballCollider != null)
        {
            ballCollider.enabled =
                true;
        }


        // Death state finish
        isDying = false;


        // Ball Paddle ke saath wait karegi
        canMove = false;


        // Extra trail safety
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
    }
}