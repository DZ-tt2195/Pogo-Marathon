using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "ScriptableObjects/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    public int levelSizeX;
    public int levelSizeY;
    public int numJewels;
    public int numFlying;
    public int numSpinners;
    public int blanked;
}
