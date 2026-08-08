using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    //// Inspector se assign hone wala Brick Prefab
    [SerializeField] private GameObject brickPrefab;

    //// Grid mein total kitni rows hongi
    //[SerializeField] private int rows = 1;

    //// Har row mein total kitni bricks hongi
    //[SerializeField] private int columns = 3;

    //// Har brick ke center ke darmiyan horizontal distance
    //[SerializeField] private float horizontalSpacing = 1.5f;

    //// Har row ke darmiyan vertical distance
    //[SerializeField] private float verticalSpacing = 0.8f;

    // Current level mein kitni bricks abhi baqi hain
    private int remainingBricks;

    public void ClearLevel()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void BuildLevel(LevelData currentLevelData)
    {
        // Agar Brick Prefab Inspector mein assign nahi hai
        // to spawning nahi karni
        //if (brickPrefab == null)
        //{
        //    Debug.LogError("Brick Prefab assign nahi hai.");
        //    return;
        //}

        // Total bricks ka count calculate karke save karna
        //
        // Example:
        // 3 rows × 5 columns = 15 bricks
        //remainingBricks = currentLevelData.rows * currentLevelData.columns;

        // First brick ke center se last brick ke center tak
        // total horizontal distance calculate karna
        //
        // columns - 1 isliye:
        // 5 bricks ke darmiyan sirf 4 gaps hote hain
        float totalWidth =
            (currentLevelData.columns - 1) * currentLevelData.horizontalSpacing;

        // Grid ko BrickSpawner ke center mein lane ke liye
        // total width ka aadha left side shift karna
        float startX =
            -totalWidth / 2f;

        int requiredSize = currentLevelData.rows * currentLevelData.columns;


        if (currentLevelData.brickLayout.Length != requiredSize)
        {
            Debug.LogError(
                "Brick Layout size must be " + requiredSize +
                ". Current size: " + currentLevelData.brickLayout.Length
            );

            return;
        }

        // Har row ko create karna
        for (int row = 0; row < currentLevelData.rows; row++)
        {
            // Current row ke andar har column ki brick create karna
            for (int column = 0; column < currentLevelData.columns; column++)
            {

                int brickIndex = row * currentLevelData.columns + column;

                //Debug.Log(brickIndex);

                // Current brick ki horizontal X position calculate karna
                //
                // startX = left side ki starting position
                // column * spacing = har next brick ko right move karna
                float xPosition =
                    startX + column * currentLevelData.horizontalSpacing;

                // Current row ki vertical Y position calculate karna
                //
                // Row 0 = 0
                // Row 1 = verticalSpacing
                // Row 2 = verticalSpacing × 2
                float yPosition =
                    row * currentLevelData.verticalSpacing;

                //// Calculated X aur Y se brick ki local position banana
                Vector2 brickPosition =
                    new Vector2(xPosition, yPosition);

                // Brick Prefab ki nayi copy create karna
                //
                // transform dene ka matlab:
                // current BrickSpawner ko brick ka parent banana

                BrickData brickData = currentLevelData.brickLayout[brickIndex];

                if(brickData == null)
                {
                    continue;
                }

                if (!brickData.unbreakable)
                {
                    remainingBricks++;
                }

                GameObject newBrick = Instantiate(brickData.prefab, transform);

                //// New Brick ko BrickSpawner ke relative position dena
                newBrick.transform.localPosition = brickPosition;

                //// Spawn hui Brick se Brick component lena
                Brick brick = newBrick.GetComponent<Brick>();

                //// Agar Brick component prefab par mojood hai
                if (brick != null)
                {
                    // Brick ko current BrickSpawner ka reference dena
                    //
                    // this = current BrickSpawner
                    brick.SetSpawner(this);

                    brick.SetData(brickData);

                }
                else
                {
                    Debug.LogError(
                        "Brick Prefab par Brick script nahi lagi."
                    );
                }
            }
        }
    }

    // Jab koi Brick destroy hone wali ho
    // to Brick script yeh method call karegi
    public void BrickDestroyed()
    {
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayBrickBreak();
        }
        // Remaining bricks ke count mein se 1 kam karna
        remainingBricks--;

        // Agar saari bricks destroy ho chuki hain
        if (remainingBricks == 0)
        {
            StartCoroutine(ShowLevelCompleteAfterDelay());
        }
    }

    private IEnumerator ShowLevelCompleteAfterDelay()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2f);
        if (UIManager.instance != null)
        {
            // Level Complete panel show karna
            UIManager.instance.ShowLevelComplete();
        }
    }
}