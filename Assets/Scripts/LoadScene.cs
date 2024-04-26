using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    Button button;
    public string sceneToLoad;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Load);
    }

    /// <summary>
    /// button that loads the next scene
    /// </summary>
    void Load()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
