using UnityEngine;

public class PaddleController : MonoBehaviour
{
    // Paddle kitni tezi se left aur right move karega
    [SerializeField] private float speed = 5f;

    // Paddle center se maximum kitna left ya right ja sakta hai
    //
    // Positive value right limit hogi
    // Negative value automatically left limit banegi
    [SerializeField] private float limitPaddleXPosition = 7.37f;

    // Keyboard se milne wali horizontal direction save hogi
    //
    // Left Arrow / A  = -1
    // Right Arrow / D = 1
    // Koi key nahi     = 0
    private float inputX;

    // Isi Paddle GameObject ke Rigidbody2D ka reference
    private Rigidbody2D rb;

    private void Awake()
    {
        // Current Paddle GameObject se Rigidbody2D component lena
        rb = GetComponent<Rigidbody2D>();

        // Agar Paddle par Rigidbody2D nahi laga
        // to Console mein error show karna
        if (rb == null)
        {
            Debug.LogError(
                "Paddle GameObject par Rigidbody2D component nahi laga."
            );
        }
    }

    private void Update()
    {
        // Har frame keyboard input read karna
        //
        // GetAxisRaw smooth value nahi deta
        // Seedha -1, 0 ya 1 deta hai
        inputX = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        // Agar Rigidbody2D available nahi hai
        // to movement code run nahi karna
        if (rb == null)
        {
            return;
        }

        // Paddle ki current X position mein movement add karna
        //
        // Direction × Speed × Fixed Time
        float xPosition =
            rb.position.x +
            inputX * speed * Time.fixedDeltaTime;

        // Paddle ki nayi position banana
        //
        // Sirf X position change hogi
        // Y position current value par same rahegi
        Vector2 newPosition =
            new Vector2(
                xPosition,
                rb.position.y
            );

        // Paddle ko allowed left aur right limits ke andar rakhna
        //
        // Example:
        // Minimum X = -7.37
        // Maximum X = 7.37
        newPosition.x = Mathf.Clamp(
            newPosition.x,
            -limitPaddleXPosition,
            limitPaddleXPosition
        );

        // Rigidbody2D physics system ke through
        // Paddle ko calculated position par move karna
        rb.MovePosition(newPosition);
    }
}