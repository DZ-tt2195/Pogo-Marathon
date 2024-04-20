using UnityEngine;

public class CarryVariables : MonoBehaviour
{
    public static CarryVariables instance;
    public LevelSettings settingsToUse;
    public PrefabDatabase prefabDB;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            settingsToUse = ScriptableObject.CreateInstance<LevelSettings>();
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
