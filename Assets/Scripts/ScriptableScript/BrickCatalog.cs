using UnityEngine;

[CreateAssetMenu(fileName = "BrickCatalog", menuName = "Brick Breaker/Brick Catalog")]
public class BrickCatalog : ScriptableObject
{
    public BrickData[] bricks;
}
