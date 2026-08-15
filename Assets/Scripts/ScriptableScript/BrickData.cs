using UnityEngine;

[CreateAssetMenu(fileName = "BrickData", menuName = "Brick Breaker/Brick Data")]
public class BrickData : ScriptableObject
{
    public GameObject prefab;
    public int hitPoints = 1;
    public int score = 10;
    public bool unbreakable;
    public Sprite damagedSprite;

}
