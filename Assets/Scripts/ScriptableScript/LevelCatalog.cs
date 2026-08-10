using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Brick Breaker/Level Data")]
public class LevelCatalog : ScriptableObject
{
    [SerializeField] private LevelData[] levelDatas;
}