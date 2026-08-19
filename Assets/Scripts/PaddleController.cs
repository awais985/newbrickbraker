using System.Collections;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    // =========================================================
    // MOVEMENT SETTINGS
    // =========================================================

    [Header("Movement Settings")]

    // Paddle kitni speed se left/right move karega
    [SerializeField] private float speed = 5f;

    // Paddle center se maximum X limit
    [SerializeField] private float limitPaddleXPosition = 7.37f;


    // =========================================================
    // POWER-UP SETTINGS
    // =========================================================

    [Header("Power Up Settings")]

    // Expand Paddle kitna wide hoga
    [SerializeField] private float expandMultiplier = 1.5f;

    // Expand effect kitni der chalega
    [SerializeField] private float expandDuration = 5f;


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // Keyboard input
    private float inputX;

    // Paddle Rigidbody2D
    private Rigidbody2D rb;

    // Starting position
    private Vector2 startPosition;

    // Starting scale
    private Vector3 originalScale;

    // Expand coroutine reference
    private Coroutine expandCoroutine;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Rigidbody2D reference lena
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError(
                "Paddle GameObject par Rigidbody2D component nahi laga."
            );
        }

        // Original scale save karna
        originalScale = transform.localScale;
    }


    private void Start()
    {
        // Paddle ki starting position save karna
        startPosition = rb != null
            ? rb.position
            : (Vector2)transform.position;
    }


    // =========================================================
    // INPUT
    // =========================================================

    private void Update()
    {
        // Left / Right input
        inputX = Input.GetAxisRaw("Horizontal");
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        // Current X + input movement
        float xPosition =
            rb.position.x +
            inputX *
            speed *
            Time.fixedDeltaTime;


        // X position ko allowed limits ke andar rakhna
        xPosition = Mathf.Clamp(
            xPosition,
            -limitPaddleXPosition,
            limitPaddleXPosition
        );


        // Nayi position banana
        Vector2 newPosition =
            new Vector2(
                xPosition,
                rb.position.y
            );


        // Physics ke through move karna
        rb.MovePosition(newPosition);
    }


    // =========================================================
    // EXPAND PADDLE POWER-UP
    // =========================================================

    public void ExpandPaddle()
    {
        // Agar pehle se expand coroutine chal rahi ho
        // to reset karke fresh duration start karna
        if (expandCoroutine != null)
        {
            StopCoroutine(expandCoroutine);
        }

        expandCoroutine =
            StartCoroutine(ExpandRoutine());
    }


    private IEnumerator ExpandRoutine()
    {
        // Paddle ko X direction mein wide karna
        transform.localScale =
            new Vector3(
                originalScale.x * expandMultiplier,
                originalScale.y,
                originalScale.z
            );


        // Temporary effect duration
        yield return new WaitForSeconds(
            expandDuration
        );


        // Original size par wapas
        transform.localScale =
            originalScale;


        expandCoroutine = null;
    }


    // =========================================================
    // RESET PADDLE
    // =========================================================

    public void ResetPaddle()
    {
        // Agar expand effect chal raha ho
        if (expandCoroutine != null)
        {
            StopCoroutine(expandCoroutine);
            expandCoroutine = null;
        }


        // Paddle size normal karna
        transform.localScale =
            originalScale;


        // Paddle ko starting position par lana
        if (rb != null)
        {
            rb.position =
                startPosition;

            rb.linearVelocity =
                Vector2.zero;
        }
        else
        {
            transform.position =
                startPosition;
        }
    }
}