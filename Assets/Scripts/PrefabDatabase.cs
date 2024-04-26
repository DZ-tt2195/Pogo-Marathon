using UnityEngine;

[CreateAssetMenu(fileName = "PrefabDB", menuName = "ScriptableObjects/PrefabDatabase", order = 1)]
public class PrefabDatabase : ScriptableObject
{
    public Maze2Cell cellPrefab; //prefab for a tile
    public Jewel jewelPrefab; //prefab for jewels
    public Spinner spinnerPrefab; //prefab for spinners
    public GameObject flyingCubePrefab; //prefab for cubes that let you fly
}