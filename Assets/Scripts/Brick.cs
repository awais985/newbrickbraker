using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    // Is Brick ko destroy karne par player ko kitna score milega
    //
    // Har Brick Prefab ke Inspector mein
    // alag score value set ki ja sakti hai
    [SerializeField] private float fadeSpeed = 4f;
    

    private SpriteRenderer spriteRenderer;
    private bool isBreaking;
    private Collider2D brickCollider;
    private BrickData brickData;
    private int currentHitPoints;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        brickCollider = GetComponent<Collider2D>();
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
            return;
        }

        currentHitPoints--;

        // Abhi HP baqi hai
        if (currentHitPoints > 0)
        {
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

        if (spriteRenderer != null)
        {
            StartCoroutine(FadeOutBrick());
        }
        else
        {
            Destroy(gameObject);
        }
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
}