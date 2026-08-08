//using UnityEngine;

//[SerializeField] private float chunksFadeSpeed = 2f;
//[SerializeField] private float chunksFadeDelay = 0.5f;
//// [SerializeField] private GameObject hitEffectPrefab;
//[SerializeField] private GameObject hitEffectPrefab;
//[SerializeField] private float breakForce = 3f;
//[SerializeField] private float maxTorque = 25f
//GameObject spawnedChunks = Instantiate(hitEffectPrefab, transform.position, transform.rotation);

//if (spawnedChunks != null)
//{
//Rigidbody2D[] chunkRigidbodies = spawnedChunks.GetComponentsInChildren<Rigidbody2D>();
//SpriteRenderer[] chunkRenderers = spawnedChunks.GetComponentsInChildren<SpriteRenderer>();
//if (chunkRigidbodies.Length > 0)
//{
//    foreach (Rigidbody2D rb in chunkRigidbodies)
//    {
//        float xDirection = Random.Range(-1f, 1f);

//        if (Mathf.Abs(xDirection) < 0.25f)
//        {
//            xDirection = xDirection < 0f ? -0.25f : 0.25f;
//        }
//        Vector2 direction = new Vector2(
//           xDirection,
//            Random.Range(0.5f, 1.5f)
//            ).normalized;

//        float randomTorque =
//                Random.Range(-maxTorque, maxTorque);
//        rb.AddForce(direction * breakForce, ForceMode2D.Impulse);
//        //rb.AddTorque(randomTorque,ForceMode2D.Impulse);
//    }
//}
//if(spriteRenderer != null)
//{
//    StartCoroutine(FadeOutBrick());
//}
//if (chunkRenderers.Length > 0)
//{
//    StartCoroutine(FadeOutChunks(chunkRenderers, spawnedChunks));
//}
//else
//{
//    Destroy(spawnedChunks);
//}
//}
// Current Brick  ko destroy karna
//}

//private IEnumerator FadeOutChunks(
//   SpriteRenderer[] chunkRenderers,
//   GameObject spawnedChunks
//)
//{
//    bool chunksVisible = true;

//    yield return new WaitForSeconds(chunksFadeDelay);


//    while (chunksVisible)
//    {
//        chunksVisible = false;

//        foreach (SpriteRenderer chunkRenderer in chunkRenderers)
//        {
//            if (chunkRenderer != null && chunkRenderer.color.a > 0f)
//            {
//                Color color = chunkRenderer.color;

//                color.a = Mathf.MoveTowards(color.a, 0f, chunksFadeSpeed * Time.deltaTime);

//                chunkRenderer.color = color;
//                chunksVisible = true;
//            }
//        }

//        yield return null;
//    }

//    Destroy(spawnedChunks);
//    Destroy(gameObject);
//}