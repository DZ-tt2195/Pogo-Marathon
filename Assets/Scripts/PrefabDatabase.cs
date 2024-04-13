using UnityEngine;

[CreateAssetMenu(fileName = "PrefabDB", menuName = "ScriptableObjects/PrefabDatabase", order = 1)]
public class PrefabDatabase : ScriptableObject
{
    public Maze2Cell cellPrefab;
    public Jewel jewelPrefab;
}