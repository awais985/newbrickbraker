using UnityEngine;

[CreateAssetMenu(fileName = "LevelData",menuName = "Brick Breaker/Level Data")]
public class LevelData : ScriptableObject
{
    public int rows = 1;
    public int columns = 3;

    public float horizontalSpacing = 1.5f;
    public float verticalSpacing = 0.8f;

    public BrickData[] brickLayout;
}