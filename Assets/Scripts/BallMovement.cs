using System.Collections;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float speedIncrease = 0.3f;
    [SerializeField] private float minimumVerticalDirection = 0.3f;
    private Vector2 lastValidDirection =  new Vector2(1f, 1f).normalized;
    // Minimum X-axis velocity ratio taakay ball straight up/down na ho
    [SerializeField] private float minimumHorizontalDirection = 0.35f;

    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float gapBetweenPaddleAndBall = 0.7f;
    [SerializeField] private Transform paddle;

    [Header("Trail")]
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Death Explosion")]
    [SerializeField] private ParticleSystem deathExplosionPrefab;
    [SerializeField] private float deathDelay = 0.35f;

    [Header("Slow Ball Power Up")]
    [SerializeField] private float slowMultiplier = 0.65f;
    [SerializeField] private float slowDuration = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D ballCollider;

    private Vector2 startDirection;
    private bool canMove;
    private bool isDying;
    private bool isExtraBall;

    private float originalSpeed;
    private Coroutine slowCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ballCollider = GetComponent<Collider2D>();

        if (trailRenderer == null)
            trailRenderer = GetComponent<TrailRenderer>();

        if (paddle == null)
        {
            GameObject paddleObject = GameObject.FindGameObjectWithTag("Paddle");
            if (paddleObject != null)
                paddle = paddleObject.transform;
        }
    }

    private void Start()
    {
        originalSpeed = speed;
        startDirection = new Vector2(1f, 1f).normalized;
        SetTrailActive(false);
    }

    private void Update()
    {
        if (isDying || canMove) return;

        if (paddle != null)
        {
            transform.position = new Vector2(
                paddle.position.x,
                paddle.position.y + gapBetweenPaddleAndBall
            );
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBall();
        }
    }

    private void FixedUpdate()
    {
        // Continuous check to fix direct Vertical/Horizontal infinite loops
        if (canMove && !isDying)
        {
            FixStraightMovement();
        }
    }

    public void LaunchBall()
    {
        if (canMove || isDying)
        {
            FixStraightMovement();
            return;
        }

        if (rb == null) return;

        canMove = true;
        SetTrailActive(true);
        rb.linearVelocity = startDirection * speed;
    }

    public void SetAsExtraBall() => isExtraBall = true;
    public bool IsExtraBall() => isExtraBall;

    public Vector2 GetCurrentVelocity() => rb == null ? Vector2.zero : rb.linearVelocity;

    public void SetVelocity(Vector2 velocity)
    {
        if (rb == null) return;

        canMove = true;
        rb.linearVelocity = velocity;
        SetTrailActive(true);
    }

    // SLOW BALL LOGIC FOR SINGLE BALL
    public void SlowBall()
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowBallRoutine());
    }

    private IEnumerator SlowBallRoutine()
    {
        speed = originalSpeed * slowMultiplier;

        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }

        yield return new WaitForSeconds(slowDuration);

        speed = originalSpeed;

        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }

        slowCoroutine = null;
    }

    // MULTI BALL CREATION
    public void CreateMultiBall()
    {
        if (rb == null) return;

        Vector2 currentVelocity = rb.linearVelocity;
        if (currentVelocity.sqrMagnitude < 0.01f) return;

        BallMovement leftBall = Instantiate(this, transform.position, Quaternion.identity);
        BallMovement rightBall = Instantiate(this, transform.position, Quaternion.identity);

        leftBall.originalSpeed = this.originalSpeed;
        leftBall.speed = this.speed;

        rightBall.originalSpeed = this.originalSpeed;
        rightBall.speed = this.speed;

        Vector2 leftDirection = Quaternion.Euler(0f, 0f, 25f) * currentVelocity.normalized;
        Vector2 rightDirection = Quaternion.Euler(0f, 0f, -25f) * currentVelocity.normalized;

        leftBall.SetAsExtraBall();
        rightBall.SetAsExtraBall();

        leftBall.SetVelocity(leftDirection * currentVelocity.magnitude);
        rightBall.SetVelocity(rightDirection * currentVelocity.magnitude);

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterBall();
            GameManager.instance.RegisterBall();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDying) return;

        if (collision.collider.CompareTag("Paddle"))
        {
            if (AudioClipManager.instance != null)
                AudioClipManager.instance.PlayPaddleHit();

            float halfWidth = collision.collider.bounds.size.x / 2f;
            float hitPosition = (transform.position.x - collision.transform.position.x) / halfWidth;
            hitPosition = Mathf.Clamp(hitPosition, -1f, 1f);

            // X-axis angle correction
            hitPosition = GetSafeHitPosition(hitPosition);

            Vector2 newDirection = new Vector2(hitPosition, 1f).normalized;
            rb.linearVelocity = newDirection * speed;
        }

        if (collision.collider.CompareTag("Boundaries"))
        {
            if (AudioClipManager.instance != null)
                AudioClipManager.instance.PlayBoundaryHit();
        }

        FixStraightMovement();
    }

    // Main Fix Function for Straight Vertical & Horizontal Angles
    private void FixStraightMovement()
    {
        if (rb == null) return;

        Vector2 currentVelocity = rb.linearVelocity;
        float currentSpeed = currentVelocity.magnitude;

        if (currentSpeed <= 0.01f) return;

        Vector2 direction = currentVelocity.normalized;

        // Check 1: Agar X-axis velocity bohot kam/zero ho (Straight Up/Down lock)
        if (Mathf.Abs(direction.x) < minimumHorizontalDirection)
        {
            float pushSign = (direction.x >= 0f) ? 1f : -1f;
            if (Mathf.Approximately(direction.x, 0f))
                pushSign = (Random.value > 0.5f) ? 1f : -1f;

            direction.x = minimumHorizontalDirection * pushSign;
            direction = direction.normalized;
        }

        // Check 2: Agar Y-axis velocity bohot kam ho (Straight Left/Right loop)
        if (Mathf.Abs(direction.y) < minimumVerticalDirection)
        {
            float pushSign = (direction.y >= 0f) ? 1f : -1f;
            direction.y = minimumVerticalDirection * pushSign;
            direction = direction.normalized;
        }

        rb.linearVelocity = direction * currentSpeed;
    }

    private float GetSafeHitPosition(float hitPosition)
    {
        // Force minimum angle on paddle collision
        if (Mathf.Abs(hitPosition) < minimumHorizontalDirection)
        {
            if (hitPosition < 0f) return -minimumHorizontalDirection;
            if (hitPosition > 0f) return minimumHorizontalDirection;

            // Random bounce angle if hit exactly center
            return (Random.value > 0.5f) ? minimumHorizontalDirection : -minimumHorizontalDirection;
        }
        return hitPosition;
    }

    private void SetTrailActive(bool active)
    {
        if (trailRenderer == null) return;
        trailRenderer.emitting = active;
        trailRenderer.Clear();
    }

    public void ExplodeBall()
    {
        if (isDying) return;
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        isDying = true;
        canMove = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        SetTrailActive(false);

        if (deathExplosionPrefab != null)
        {
            ParticleSystem explosion = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, 2f);
        }

        if (CameraShake.instance != null)
            CameraShake.instance.Shake();

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (ballCollider != null) ballCollider.enabled = false;

        yield return new WaitForSeconds(deathDelay);

        if (GameManager.instance != null)
            GameManager.instance.UnregisterBall(this);
        else
            ResetBall();
    }

    public void ResetBall()
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }

        speed = originalSpeed;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        SetTrailActive(false);

        if (paddle != null)
        {
            transform.position = new Vector2(
                paddle.position.x,
                paddle.position.y + gapBetweenPaddleAndBall
            );
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (ballCollider != null) ballCollider.enabled = true;

        isDying = false;
        canMove = false;

        if (trailRenderer != null) trailRenderer.Clear();
    }
}