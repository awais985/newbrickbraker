using System.Collections;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    // =========================================================
    // MOVEMENT SETTINGS
    // =========================================================

    [Header("Movement Settings")]

    // Paddle kitni speed se keyboard par move karega
    [SerializeField] private float speed = 5f;

    // Paddle center se maximum X limit
    [SerializeField] private float limitPaddleXPosition = 7.37f;


    // =========================================================
    // MOBILE TOUCH SETTINGS
    // =========================================================

    [Header("Mobile Touch Settings")]

    // Mobile touch movement enable / disable
    [SerializeField] private bool enableTouchMovement = true;

    // Touch movement sensitivity
    //
    // 1 = finger ke movement ke barabar
    // 1.2 = thora faster
    // 0.8 = thora slower
    [SerializeField] private float touchSensitivity = 1f;


    // =========================================================
    // POWER-UP SETTINGS
    // =========================================================

    [Header("Power Up Settings")]

    // Expand Paddle kitna wide hoga
    [SerializeField] private float expandMultiplier = 1.5f;

    // Shrink Paddle kitna chota hoga
    [SerializeField] private float shrinkMultiplier = 0.7f;

    // Expand / Shrink effect kitni der chalega
    [SerializeField] private float expandDuration = 5f;

    // Resize animation speed
    [SerializeField] private float resizeAnimationDuration = 0.15f;


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // Keyboard input
    private float inputX;

    // Paddle Rigidbody2D
    private Rigidbody2D rb;

    // Main Camera
    private Camera mainCamera;

    // Starting position
    private Vector2 startPosition;

    // Starting scale
    private Vector3 originalScale;

    // Expand coroutine reference
    private Coroutine expandCoroutine;

    // Shrink coroutine reference
    private Coroutine shrinkCoroutine;


    // =========================================================
    // TOUCH RUNTIME DATA
    // =========================================================

    // Kya finger currently screen par drag kar raha hai
    private bool isTouching;

    // Previous touch world X position
    private float previousTouchWorldX;

    // Touch se calculate hui movement
    private float touchMovementX;


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


        // Main Camera reference
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "PaddleController: Main Camera nahi mili. Camera ko MainCamera tag dein."
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
        // Keyboard input
        HandleKeyboardInput();

        // Mobile touch input
        HandleTouchInput();
    }


    // =========================================================
    // KEYBOARD INPUT
    // =========================================================

    private void HandleKeyboardInput()
    {
        // Agar mobile touch active hai
        // to keyboard movement temporarily ignore karna
        if (isTouching)
        {
            inputX = 0f;
            return;
        }


        // Left / Right input
        inputX = Input.GetAxisRaw("Horizontal");
    }


    // =========================================================
    // TOUCH INPUT
    // =========================================================

    private void HandleTouchInput()
    {
        if (!enableTouchMovement)
        {
            isTouching = false;
            touchMovementX = 0f;
            return;
        }


        // Agar screen par koi touch nahi hai
        if (Input.touchCount == 0)
        {
            isTouching = false;
            touchMovementX = 0f;
            return;
        }


        // First finger use karna
        Touch touch = Input.GetTouch(0);


        // -----------------------------------------------------
        // TOUCH START
        // -----------------------------------------------------

        if (touch.phase == TouchPhase.Began)
        {
            isTouching = true;

            previousTouchWorldX =
                ScreenToWorldX(
                    touch.position
                );

            touchMovementX = 0f;
        }


        // -----------------------------------------------------
        // TOUCH MOVING
        // -----------------------------------------------------

        else if (
            touch.phase == TouchPhase.Moved ||
            touch.phase == TouchPhase.Stationary
        )
        {
            isTouching = true;


            float currentTouchWorldX =
                ScreenToWorldX(
                    touch.position
                );


            // Finger ne kitna horizontal move kiya
            float difference =
                currentTouchWorldX -
                previousTouchWorldX;


            // Sensitivity apply karna
            touchMovementX =
                difference *
                touchSensitivity;


            // Current ko next frame ke liye save karna
            previousTouchWorldX =
                currentTouchWorldX;
        }


        // -----------------------------------------------------
        // TOUCH END
        // -----------------------------------------------------

        else if (
            touch.phase == TouchPhase.Ended ||
            touch.phase == TouchPhase.Canceled
        )
        {
            isTouching = false;

            touchMovementX = 0f;
        }
    }


    // =========================================================
    // SCREEN POSITION → WORLD X
    // =========================================================

    private float ScreenToWorldX(
        Vector2 screenPosition)
    {
        if (mainCamera == null)
        {
            return 0f;
        }


        Vector3 screenPoint =
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                Mathf.Abs(
                    mainCamera.transform.position.z -
                    transform.position.z
                )
            );


        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(
                screenPoint
            );


        return worldPosition.x;
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


        float xPosition;


        // =====================================================
        // MOBILE TOUCH MOVEMENT
        // =====================================================

        if (isTouching)
        {
            xPosition =
                rb.position.x +
                touchMovementX;


            // Touch delta sirf ek physics frame use karna
            touchMovementX = 0f;
        }


        // =====================================================
        // KEYBOARD MOVEMENT
        // =====================================================

        else
        {
            xPosition =
                rb.position.x +
                inputX *
                speed *
                Time.fixedDeltaTime;
        }


        // =====================================================
        // CLAMP POSITION
        // =====================================================

        xPosition =
            Mathf.Clamp(
                xPosition,
                -limitPaddleXPosition,
                limitPaddleXPosition
            );


        // New position
        Vector2 newPosition =
            new Vector2(
                xPosition,
                rb.position.y
            );


        // Physics movement
        rb.MovePosition(
            newPosition
        );
    }


    // =========================================================
    // EXPAND PADDLE POWER-UP
    // =========================================================

    public void ExpandPaddle()
    {
        // Agar shrink chal raha hai
        // to usko pehle stop karna
        if (shrinkCoroutine != null)
        {
            StopCoroutine(
                shrinkCoroutine
            );

            shrinkCoroutine = null;
        }


        // Agar pehle se expand coroutine chal rahi ho
        // to fresh duration start karna
        if (expandCoroutine != null)
        {
            StopCoroutine(
                expandCoroutine
            );
        }


        expandCoroutine =
            StartCoroutine(
                ExpandRoutine()
            );
    }


    // =========================================================
    // SHRINK PADDLE POWER-UP
    // =========================================================

    // IMPORTANT:
    // Tumhari existing method ka same naam rakha hai
    // taa ke existing PowerUp code break na ho.
    public void ShinkPaddle()
    {
        // Agar expand chal raha hai
        // to usko pehle stop karna
        if (expandCoroutine != null)
        {
            StopCoroutine(
                expandCoroutine
            );

            expandCoroutine = null;
        }


        if (shrinkCoroutine != null)
        {
            StopCoroutine(
                shrinkCoroutine
            );
        }


        shrinkCoroutine =
            StartCoroutine(
                ShrinkRoutine()
            );
    }


    // =========================================================
    // OPTIONAL CORRECT SPELLING
    // =========================================================

    // Agar future mein correct spelling use karni ho
    // to ye bhi available hai.
    //
    // Existing ShinkPaddle() bhi kaam karti rahegi.
    public void ShrinkPaddle()
    {
        ShinkPaddle();
    }


    // =========================================================
    // EXPAND ROUTINE
    // =========================================================

    private IEnumerator ExpandRoutine()
    {
        Vector3 startScale =
            transform.localScale;


        Vector3 targetScale =
            new Vector3(
                originalScale.x *
                expandMultiplier,

                originalScale.y,

                originalScale.z
            );


        // ==========================================
        // SMOOTHLY EXPAND
        // ==========================================

        float time = 0f;


        while (time < resizeAnimationDuration)
        {
            time += Time.deltaTime;


            float t =
                time /
                resizeAnimationDuration;


            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    targetScale,
                    t
                );


            yield return null;
        }


        transform.localScale =
            targetScale;


        // ==========================================
        // EXPAND ACTIVE
        // ==========================================

        yield return new WaitForSeconds(
            expandDuration
        );


        // ==========================================
        // RETURN TO NORMAL
        // ==========================================

        startScale =
            transform.localScale;


        time = 0f;


        while (time < resizeAnimationDuration)
        {
            time += Time.deltaTime;


            float t =
                time /
                resizeAnimationDuration;


            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    originalScale,
                    t
                );


            yield return null;
        }


        transform.localScale =
            originalScale;


        expandCoroutine = null;
    }


    // =========================================================
    // SHRINK ROUTINE
    // =========================================================

    private IEnumerator ShrinkRoutine()
    {
        // Starting size
        Vector3 startScale =
            transform.localScale;


        // Target shrink size
        Vector3 targetScale =
            new Vector3(
                originalScale.x *
                shrinkMultiplier,

                originalScale.y,

                originalScale.z
            );


        // ==========================================
        // SMOOTHLY SHRINK
        // ==========================================

        float time = 0f;


        while (time < resizeAnimationDuration)
        {
            time += Time.deltaTime;


            float t =
                time /
                resizeAnimationDuration;


            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    targetScale,
                    t
                );


            yield return null;
        }


        // Exact target
        transform.localScale =
            targetScale;


        // ==========================================
        // SHRINK EFFECT ACTIVE
        // ==========================================

        yield return new WaitForSeconds(
            expandDuration
        );


        // ==========================================
        // RETURN TO NORMAL
        // ==========================================

        startScale =
            transform.localScale;


        time = 0f;


        while (time < resizeAnimationDuration)
        {
            time += Time.deltaTime;


            float t =
                time /
                resizeAnimationDuration;


            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    originalScale,
                    t
                );


            yield return null;
        }


        transform.localScale =
            originalScale;


        shrinkCoroutine = null;
    }


    // =========================================================
    // RESET PADDLE
    // =========================================================

    public void ResetPaddle()
    {
        // Expand stop
        if (expandCoroutine != null)
        {
            StopCoroutine(
                expandCoroutine
            );

            expandCoroutine = null;
        }


        // Shrink stop
        if (shrinkCoroutine != null)
        {
            StopCoroutine(
                shrinkCoroutine
            );

            shrinkCoroutine = null;
        }


        // Paddle size normal
        transform.localScale =
            originalScale;


        // Paddle starting position
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


        // Inputs reset
        inputX = 0f;

        isTouching = false;

        touchMovementX = 0f;
    }
}