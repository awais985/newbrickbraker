using UnityEngine;

public class BrickClickTest : MonoBehaviour
{
    private void OnMouseDown()
    {
        // Testing only:
        // Jis brick par mouse click hoga,
        // sirf wahi brick break hogi.

        Brick brick = GetComponent<Brick>();

        if (brick != null)
        {
            // Brick.cs mein BreakBrick()
            // private hai, isliye testing ke liye
            // ek public method banana hoga.
            brick.TestBreakBrick();
        }
    }
}