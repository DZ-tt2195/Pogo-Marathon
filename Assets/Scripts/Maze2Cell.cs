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
    [SerializeField] List<GameObject> listOfWalls = new();

    public void Init(int x, int y)
    {
        locX = x;
        locY = y;
        this.name = $"{x}, {y}";

        foreach (GameObject wall in listOfWalls)
            wall.SetActive(false);

        for (int k = 0; k < 1; k++)
        {
            int randomWall = Random.Range(0, this.listOfWalls.Count);
            this.listOfWalls[randomWall].SetActive(true);
            this.listOfWalls.RemoveAt(randomWall);
        }
    }

    public IEnumerator ShrinkAway()
    {
        foreach (Transform child in this.transform)
            child.gameObject.SetActive(false);

        float elapsedTime = 0f;
        float waitTime = 2.5f;
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
