using System.Collections;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // Current level mein kitni breakable bricks abhi baqi hain
    //
    // Unbreakable bricks is count mein include nahi hongi
    private int remainingBricks;

    // Level complete coroutine duplicate start hone se rokne ke liye
    private bool levelCompleteStarted;


    // =========================================================
    // CLEAR CURRENT LEVEL
    // =========================================================

    public void ClearLevel()
    {
        // Current BrickSpawner ke saare child bricks
        // reverse order mein destroy karna
        //
        // Reverse loop safer hota hai jab children remove ho rahe hon
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(
                transform.GetChild(i).gameObject
            );
        }

        // New level ke liye counters reset
        remainingBricks = 0;
        levelCompleteStarted = false;
    }


    // =========================================================
    // BUILD LEVEL
    // =========================================================

    public void BuildLevel(LevelData currentLevelData)
    {
        // Har new level build par count reset karna
        remainingBricks = 0;

        // Level complete state reset karna
        levelCompleteStarted = false;


        // -----------------------------------------------------
        // SAFETY CHECKS
        // -----------------------------------------------------

        // Agar LevelData hi missing hai
        if (currentLevelData == null)
        {
            Debug.LogError(
                "BrickSpawner: LevelData assign nahi hai."
            );

            return;
        }


        // Agar brick layout array missing hai
        if (currentLevelData.brickLayout == null)
        {
            Debug.LogError(
                "BrickSpawner: Brick Layout missing hai."
            );

            return;
        }


        // Rows × Columns se required array size calculate karna
        int requiredSize =
            currentLevelData.rows *
            currentLevelData.columns;


        // Check karna ke Brick Layout ka size
        // grid ke required size ke equal hai
        if (currentLevelData.brickLayout.Length != requiredSize)
        {
            Debug.LogError(
                "Brick Layout size must be " +
                requiredSize +
                ". Current size: " +
                currentLevelData.brickLayout.Length
            );

            return;
        }


        // -----------------------------------------------------
        // GRID HORIZONTAL POSITION
        // -----------------------------------------------------

        // First brick ke center se last brick ke center tak
        // total horizontal distance
        //
        // Example:
        // 5 columns = 4 gaps
        float totalWidth =
            (currentLevelData.columns - 1) *
            currentLevelData.horizontalSpacing;


        // Grid ko BrickSpawner ke center mein align karna
        //
        // Example:
        // total width = 8
        // startX = -4
        float startX =
            -totalWidth / 2f;


        // =====================================================
        // CREATE ALL BRICKS
        // =====================================================

        for (int row = 0;
             row < currentLevelData.rows;
             row++)
        {
            for (int column = 0;
                 column < currentLevelData.columns;
                 column++)
            {
                // -------------------------------------------------
                // ARRAY INDEX
                // -------------------------------------------------

                // 2D row/column ko 1D array index mein convert karna
                //
                // Example:
                // columns = 5
                //
                // row 0, column 0 → index 0
                // row 0, column 1 → index 1
                // row 1, column 0 → index 5
                int brickIndex =
                    row *
                    currentLevelData.columns +
                    column;


                // -------------------------------------------------
                // BRICK DATA
                // -------------------------------------------------

                BrickData brickData =
                    currentLevelData.brickLayout[
                        brickIndex
                    ];


                // Null ka matlab:
                // is grid position par koi brick nahi hogi
                if (brickData == null)
                {
                    continue;
                }


                // Agar prefab missing hai
                if (brickData.prefab == null)
                {
                    Debug.LogError(
                        "BrickSpawner: BrickData prefab missing at index " +
                        brickIndex
                    );

                    continue;
                }


                // -------------------------------------------------
                // BRICK POSITION
                // -------------------------------------------------

                // Horizontal position
                float xPosition =
                    startX +
                    column *
                    currentLevelData.horizontalSpacing;


                // Vertical position
                float yPosition =
                    row *
                    currentLevelData.verticalSpacing;


                // Final local position
                Vector2 brickPosition =
                    new Vector2(
                        xPosition,
                        yPosition
                    );


                // -------------------------------------------------
                // REMAINING BRICK COUNT
                // -------------------------------------------------

                // Sirf breakable bricks count karni hain
                //
                // Unbreakable brick destroy nahi hoti,
                // isliye level completion count mein include nahi hogi
                if (!brickData.unbreakable)
                {
                    remainingBricks++;
                }


                // -------------------------------------------------
                // SPAWN BRICK
                // -------------------------------------------------

                // Brick prefab instantiate karna
                //
                // transform dene ka matlab:
                // BrickSpawner iska parent hoga
                GameObject newBrick =
                    Instantiate(
                        brickData.prefab,
                        transform
                    );


                // BrickSpawner ke relative
                // calculated position assign karna
                newBrick.transform.localPosition =
                    brickPosition;


                // -------------------------------------------------
                // BRICK SCRIPT SETUP
                // -------------------------------------------------

                Brick brick =
                    newBrick.GetComponent<Brick>();


                if (brick != null)
                {
                    // Current BrickSpawner ka reference dena
                    brick.SetSpawner(this);

                    // Is brick ka BrickData dena
                    brick.SetData(brickData);
                }
                else
                {
                    Debug.LogError(
                        "BrickSpawner: Spawned prefab par Brick script nahi lagi."
                    );
                }
            }
        }


        // Debugging ke liye useful
        //Debug.Log(
        //    "Breakable Bricks: " +
        //    remainingBricks
        //);


        // Agar level mein ek bhi breakable brick nahi hai
        // to level immediately complete treat karna
        if (remainingBricks == 0)
        {
            StartLevelCompleteSequence();
        }
    }


    // =========================================================
    // BRICK DESTROYED
    // =========================================================

    // Jab koi breakable Brick destroy hoti hai
    // Brick.cs is method ko call karegi
    public void BrickDestroyed()
    {
        // Agar count already 0 hai
        // to duplicate calls ignore karna
        if (remainingBricks <= 0)
        {
            return;
        }


        // Ek brick kam
        remainingBricks--;


        // Brick break sound
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance
                .PlayBrickBreak();
        }


        //Debug.Log(
        //    "Remaining Bricks: " +
        //    remainingBricks
        //);


        // Agar saari breakable bricks destroy ho gayi
        if (remainingBricks == 0)
        {
            StartLevelCompleteSequence();
        }
    }


    // =========================================================
    // START LEVEL COMPLETE SEQUENCE
    // =========================================================

    private void StartLevelCompleteSequence()
    {
        // Same sequence multiple baar start na ho
        if (levelCompleteStarted)
        {
            return;
        }

        levelCompleteStarted = true;

        StartCoroutine(
            NextLevelDelay()
        );
    }


    // =========================================================
    // LEVEL COMPLETE DELAY
    // =========================================================

    private IEnumerator NextLevelDelay()
    {
        // Last brick break hone ke baad
        // short delay
        yield return new WaitForSeconds(1f);


        // -----------------------------------------------------
        // UNLOCK NEXT LEVEL
        // -----------------------------------------------------

        if (LevelManager.instance != null)
        {
            // Check karna ke next level available hai
            bool isNext =
                LevelManager.instance
                    .CheckNextLevelAvailable();


            // Agar next level available hai
            // to usko unlock karna
            if (isNext)
            {
                LevelManager.instance
                    .UnlockNextLevel();
            }
        }


        // Level complete panel show sequence start
        yield return StartCoroutine(
            ShowLevelCompleteAfterDelay()
        );
    }


    // =========================================================
    // SHOW LEVEL COMPLETE UI
    // =========================================================

    private IEnumerator ShowLevelCompleteAfterDelay()
    {
        // Gameplay freeze karna
        Time.timeScale = 0f;


        // Realtime wait use karna zaroori hai,
        // kyun ke Time.timeScale = 0 hai
        yield return new WaitForSecondsRealtime(
            2f
        );


        // Level Complete panel show karna
        if (UIManager.instance != null)
        {
            UIManager.instance
                .ShowLevelComplete();
        }
    }


    // =========================================================
    // OPTIONAL GETTER
    // =========================================================

    // Agar UI/debugging ko remaining bricks chahiye hon
    public int GetRemainingBricks()
    {
        return remainingBricks;
    }
}