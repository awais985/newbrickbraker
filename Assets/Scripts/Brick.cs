using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    // =========================================================
    // HIT EFFECT SETTINGS
    // =========================================================

    [Header("Hit Effect")]

    // Ball hit hone par Brick halka sa shrink hoga
    [SerializeField] private float punchScale = 0.94f;

    // Punch effect kitni der chalega
    [SerializeField] private float punchDuration = 0.08f;


    // =========================================================
    // BREAK SHARD SETTINGS
    // =========================================================

    [Header("Break Shards")]

    // Brick tootne par spawn hone wale shard prefabs
    [SerializeField] private GameObject[] shardPrefabs;

    // Ek Brick tootne par kitne shards spawn honge
    [SerializeField] private int shardCount = 5;

    // Shards kitni force ke saath bahar jayenge
    [SerializeField] private float shardForce = 3f;

    // Shards kitni rotation force lenge
    [SerializeField] private float shardTorque = 180f;

    // Shards kitni der baad destroy honge
    [SerializeField] private float shardLifetime = 1f;

    // Spawn hone wale shards ka size
    [SerializeField] private float shardScale = 0.15f;

    // Game mein currently active PowerUp
    // Ek waqt mein sirf ek PowerUp allow hoga
    private static GameObject activePowerUp;

    // =========================================================
    // POWER-UP SETTINGS
    // =========================================================

    [Header("Power Up Drop")]

    // Kaun kaun se PowerUp prefabs drop ho sakte hain
    [SerializeField] private GameObject[] powerUpPrefabs;

    // 0.2 = 20% chance
    [SerializeField, Range(0f, 1f)]
    private float powerUpDropChance = 0.2f;


    // =========================================================
    // PRIVATE REFERENCES / RUNTIME DATA
    // =========================================================

    private SpriteRenderer spriteRenderer;
    private Collider2D brickCollider;

    // Brick ki original scale
    private Vector3 originalScale;

    // Hit punch coroutine reference
    private Coroutine punchCoroutine;

    // Check karega Brick final break process mein hai ya nahi
    private bool isBreaking;

    // ScriptableObject data for this Brick
    private BrickData brickData;

    // Current remaining HP
    private int currentHitPoints;

    // Is Brick ko spawn karne wale BrickSpawner ka reference
    private BrickSpawner brickSpawner;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Isi Brick ka SpriteRenderer lena
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Isi Brick ka Collider2D lena
        brickCollider = GetComponent<Collider2D>();

        // Starting scale save karna
        originalScale = transform.localScale;
    }

    // BrickSpawner / level setup se BrickData receive karna
    public void SetData(BrickData data)
    {
        brickData = data;

        // Brick ki starting HP set karna
        currentHitPoints = brickData.hitPoints;
    }


    // BrickSpawner apna reference yahan bhejega
    public void SetSpawner(BrickSpawner spawner)
    {
        brickSpawner = spawner;
    }


    // =========================================================
    // COLLISION
    // =========================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sirf Ball ke collision ko process karna
        if (!collision.collider.CompareTag("Ball"))
        {
            return;
        }

        // Agar Brick already break ho rahi hai
        // to duplicate hit process nahi karna
        if (isBreaking)
        {
            return;
        }

        // Safety check:
        // Agar BrickData assign nahi hai
        if (brickData == null)
        {
            return;
        }


        // =====================================================
        // UNBREAKABLE BRICK
        // =====================================================

        if (brickData.unbreakable)
        {
            // Chhota punch effect
            PlayHitPunch();

            // White flash
            StartCoroutine(SolidHitFlash());

            // Future solid brick sound:
            /*
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance.PlaySolidBrickHit();
            }
            */

            return;
        }


        // =====================================================
        // NORMAL BRICK DAMAGE
        // =====================================================

        // Har Ball hit par 1 HP kam
        currentHitPoints--;


        // Agar Brick ki HP abhi bhi baqi hai
        if (currentHitPoints > 0)
        {
            // Hit animation
            PlayHitPunch();

            // Damaged / cracked sprite show karna
            if (brickData.damagedSprite != null &&
                spriteRenderer != null)
            {
                spriteRenderer.sprite =
                    brickData.damagedSprite;
            }

            // Future hit sound:
            /*
            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance.PlayBrickHit();
            }
            */

            return;
        }


        // =====================================================
        // FINAL HIT / BRICK DESTROY
        // =====================================================

        BreakBrick();
    }


    // =========================================================
    // FINAL BRICK BREAK
    // =========================================================

    private void BreakBrick()
    {
        // Duplicate break calls ko rokna
        isBreaking = true;


        // Brick ka collider turant disable karna
        // taake Ball same Brick ko dobara hit na kare
        if (brickCollider != null)
        {
            brickCollider.enabled = false;
        }


        // Player ko score dena
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(
                brickData.score
            );
        }


        // BrickSpawner ko batana ke
        // ek breakable Brick destroy ho gayi
        if (brickSpawner != null)
        {
            brickSpawner.BrickDestroyed();
        }


        // Brick ke pieces spawn karna
        SpawnShards();


        // Random chance se PowerUp drop karna
        TryDropPowerUp();


        // Main Brick object remove karna
        Destroy(gameObject);
    }


    public void TestBreakBrick()
    {
        // Testing only
        if (isBreaking)
        {
            return;
        }

        if (brickData == null)
        {
            return;
        }

        // Unbreakable brick ko testing mein bhi na todo
        if (brickData.unbreakable)
        {
            return;
        }

        BreakBrick();
    }

    // =========================================================
    // HIT PUNCH EFFECT
    // =========================================================

    private void PlayHitPunch()
    {
        // Agar previous punch coroutine chal rahi ho
        // to pehle usko stop karna
        if (punchCoroutine != null)
        {
            StopCoroutine(punchCoroutine);
        }

        // Fresh punch animation start
        punchCoroutine = StartCoroutine(
            HitPunch()
        );
    }


    private IEnumerator HitPunch()
    {
        // Brick ko temporarily thora shrink karna
        transform.localScale =
            originalScale * punchScale;

        // TimeScale ko ignore karke short wait
        yield return new WaitForSecondsRealtime(
            punchDuration
        );

        // Original size restore
        transform.localScale =
            originalScale;

        punchCoroutine = null;
    }


    // =========================================================
    // POWER-UP DROP
    // =========================================================

    private void TryDropPowerUp()
    {
        // =====================================================
        // CHECK ACTIVE POWER-UP
        // =====================================================

        // Agar already koi PowerUp screen par mojood hai
        // to naya PowerUp spawn nahi karna
        if (activePowerUp != null)
        {
            return;
        }


        // =====================================================
        // SAFETY CHECK
        // =====================================================

        // PowerUp prefabs assign nahi hain
        if (powerUpPrefabs == null ||
            powerUpPrefabs.Length == 0)
        {
            return;
        }


        // =====================================================
        // DROP CHANCE
        // =====================================================

        // Example:
        // 0.2 = 20% chance
        if (Random.value > powerUpDropChance)
        {
            return;
        }


        // =====================================================
        // SELECT RANDOM POWER-UP
        // =====================================================

        GameObject selectedPowerUp =
            powerUpPrefabs[
                Random.Range(
                    0,
                    powerUpPrefabs.Length
                )
            ];


        // Prefab missing ho to spawn nahi karna
        if (selectedPowerUp == null)
        {
            return;
        }


        // =====================================================
        // SPAWN POWER-UP
        // =====================================================

        // Active PowerUp ka reference save karna
        activePowerUp =
            Instantiate(
                selectedPowerUp,
                transform.position,
                Quaternion.identity
            );

    }


    // =========================================================
    // SHARD BREAK EFFECT
    // =========================================================

    private void SpawnShards()
    {
        // Agar shard prefabs assign nahi hain
        if (shardPrefabs == null ||
            shardPrefabs.Length == 0)
        {
            return;
        }


        // Required quantity ke mutabiq shards spawn karna
        for (int i = 0; i < shardCount; i++)
        {
            // Random shard prefab choose karna
            GameObject selectedPrefab =
                shardPrefabs[
                    Random.Range(
                        0,
                        shardPrefabs.Length
                    )
                ];


            // Null prefab skip karna
            if (selectedPrefab == null)
            {
                continue;
            }


            // Random rotation ke saath shard spawn
            GameObject shard =
                Instantiate(
                    selectedPrefab,
                    transform.position,
                    Quaternion.Euler(
                        0f,
                        0f,
                        Random.Range(
                            0f,
                            360f
                        )
                    )
                );


            // Shard ka desired size set karna
            shard.transform.localScale =
                Vector3.one * shardScale;


            // Rigidbody2D lena
            Rigidbody2D shardRb =
                shard.GetComponent<Rigidbody2D>();


            if (shardRb != null)
            {
                // Random upward/sideways direction
                Vector2 randomDirection =
                    new Vector2(
                        Random.Range(-1f, 1f),
                        Random.Range(0.3f, 1f)
                    ).normalized;


                // Thori random variation ke saath
                // shard ko impulse dena
                float randomForce =
                    Random.Range(
                        shardForce * 0.7f,
                        shardForce * 1.3f
                    );


                shardRb.AddForce(
                    randomDirection * randomForce,
                    ForceMode2D.Impulse
                );


                // Random clockwise /
                // counter-clockwise rotation
                shardRb.AddTorque(
                    Random.Range(
                        -shardTorque,
                        shardTorque
                    )
                );
            }


            // Kuch time baad shard remove
          
        }
    }


    // =========================================================
    // UNBREAKABLE BRICK FLASH
    // =========================================================

    private IEnumerator SolidHitFlash()
    {
        // Safety check
        if (spriteRenderer == null)
        {
            yield break;
        }


        // Current original color save
        Color originalColor = spriteRenderer.color;


        // Temporary white flash
        spriteRenderer.color = Color.white;


        // Very short wait
        yield return new WaitForSecondsRealtime(
            0.08f
        );


        // Original color restore
        spriteRenderer.color =
            originalColor;
    }
}