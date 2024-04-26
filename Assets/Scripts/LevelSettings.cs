using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "ScriptableObjects/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    public int levelSizeX; //size of the level
    public int levelSizeY;
    public int numJewels; //jewels to spawn in the level
    public int numFlying; //flying cubes to spawn in the level
    public int numSpinners; //spinners to spawn in the level
    public int blanked; //blank tiles in the level
    [TextArea]
    public string tutorial; //tutorial text on the menu screen
}
