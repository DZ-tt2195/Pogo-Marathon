using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Maze2TunnelDirectionIndicator { None, Left, Right, Up, Down };
public class Maze2Cell : MonoBehaviour
{
    public Maze2TunnelDirectionIndicator tunnelDirection = Maze2TunnelDirectionIndicator.None;

    [HideInInspector]
    public bool isVisited = false;

    public float mazeSize = 5;

    public int locX;
    public int locY;

    public bool shrinking = false;
    public List<GameObject> listOfWalls = new();

    public void Init(int x, int y)
    {
        locX = x;
        locY = y;
        this.name = $"{x}, {y}";
        foreach (GameObject wall in listOfWalls)
            wall.SetActive(false);
    }

    public IEnumerator ShrinkAway()
    {
        float elapsedTime = 0f;
        float waitTime = 2f;
        Vector3 maxSize = transform.localScale;

        while (elapsedTime < waitTime) //shrink until it reaches 0
        {
            transform.localScale = Vector3.Lerp(maxSize, Vector3.zero, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
