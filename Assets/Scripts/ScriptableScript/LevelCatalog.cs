using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Brick Breaker/Level Catalog")]
public class LevelCatalog : ScriptableObject
{
    public LevelData[] levels;
}