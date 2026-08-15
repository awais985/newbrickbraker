using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    // Is Brick ko destroy karne par player ko kitna score milega
    //
    // Har Brick Prefab ke Inspector mein
    // alag score value set ki ja sakti hai
    [SerializeField] private float fadeSpeed = 4f;
    [SerializeField] private float punchScale = 0.94f;
    [SerializeField] private float punchDuration = 0.08f;

    private Vector3 originalScale;
    private Coroutine punchCoroutine;
    private SpriteRenderer spriteRenderer;
    private bool isBreaking;
    private Collider2D brickCollider;
    private BrickData brickData;
    private int currentHitPoints;

    [Header("Break Shards")]
    [SerializeField] private GameObject[] shardPrefabs;
    [SerializeField] private int shardCount = 5;
    [SerializeField] private float shardForce = 3f;
    [SerializeField] private float shardTorque = 180f;
    [SerializeField] private float shardLifetime = 1f;
    [SerializeField] private float shardScale = 0.15f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        brickCollider = GetComponent<Collider2D>();
        originalScale = transform.localScale;
    }
    private void PlayHitPunch()
    {
        if (punchCoroutine != null)
            StopCoroutine(punchCoroutine);

        punchCoroutine = StartCoroutine(HitPunch());
    }

    private IEnumerator HitPunch()
    {
        transform.localScale = originalScale * punchScale;

        yield return new WaitForSecondsRealtime(punchDuration);

        transform.localScale = originalScale;
        punchCoroutine = null;
    }
    public void SetData(BrickData data)
    {
        brickData = data;
        currentHitPoints = brickData.hitPoints;

    }

    // Us BrickSpawner ka reference
    // jis ne current Brick ko create kiya
    private BrickSpawner brickSpawner;

    // BrickSpawner jab is Brick ko spawn karega
    // to apna reference is method ke through bhejega
    public void SetSpawner(BrickSpawner spawner)
    {
        // Mile hue BrickSpawner reference ko save karna
        brickSpawner = spawner;
    }

    // Jab koi object Brick ke saath collide kare
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ball"))
        {
            return;
        }

        if (isBreaking)
        {
            return;
        }

        if (brickData == null)
        {
            return;
        }

        // Solid / unbreakable brick
        if (brickData.unbreakable)
        {
            PlayHitPunch();

            if (AudioClipManager.instance != null)
            {
                //AudioClipManager.instance.PlaySolidBrickHit();
            }
            StartCoroutine(SolidHitFlash());

            return;
        }

        currentHitPoints--;

        // Abhi HP baqi hai
        if (currentHitPoints > 0)
        {
            PlayHitPunch();
            if (AudioClipManager.instance != null)
            {
             //   AudioClipManager.instance.PlayBrickHit();
            }
            if (brickData.damagedSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = brickData.damagedSprite;
            }
            return;
        }

        // HP 0 ho gayi
        isBreaking = true;

        if (brickCollider != null)
        {
            brickCollider.enabled = false;
        }

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(brickData.score);
        }

        if (brickSpawner != null)
        {
            brickSpawner.BrickDestroyed();
        }

        SpawnShards();


        Destroy(gameObject);

        //if (spriteRenderer != null)
        //{
        //    StartCoroutine(FadeOutBrick());
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    private IEnumerator FadeOutBrick()
    {
        while (spriteRenderer.color.a > 0f)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.MoveTowards(color.a, 0f, fadeSpeed * Time.unscaledDeltaTime);

            spriteRenderer.color = color;

            yield return null;

        }

        Destroy(gameObject);
    }

    private void SpawnShards()
    {
        if (shardPrefabs == null || shardPrefabs.Length == 0)
        {
            return;
        }

        for (int i = 0; i < shardCount; i++)
        {
            GameObject selectedPrefab =
                shardPrefabs[
                    Random.Range(0, shardPrefabs.Length)
                ];

            if (selectedPrefab == null)
            {
                continue;
            }

            GameObject shard = Instantiate(
                selectedPrefab,
                transform.position,
                Quaternion.Euler(
                    0f,
                    0f,
                    Random.Range(0f, 360f)
                )
            );

            shard.transform.localScale = Vector2.one * shardScale;

            Rigidbody2D shardRb =
                shard.GetComponent<Rigidbody2D>();

            if (shardRb != null)
            {
                Vector2 randomDirection =
                    new Vector2(
                        Random.Range(-1f, 1f),
                        Random.Range(0.3f, 1f)
                    ).normalized;

                shardRb.AddForce(
                    randomDirection *
                    Random.Range(
                        shardForce * 0.7f,
                        shardForce * 1.3f
                    ),
                    ForceMode2D.Impulse
                );

                shardRb.AddTorque(
                    Random.Range(
                        -shardTorque,
                        shardTorque
                    )
                );
            }

            Destroy(
                shard,
                shardLifetime
            );
        }
    }
    private IEnumerator SolidHitFlash()
    {
        if (spriteRenderer == null)
            yield break;
        Color originalColor = spriteRenderer.color;

        spriteRenderer.color = Color.white;

        yield return new WaitForSecondsRealtime(0.08f);

        spriteRenderer.color = originalColor;
    }
}