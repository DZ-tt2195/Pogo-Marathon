using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Maze2Generator : MonoBehaviour
{
    public static Maze2Generator instance;
	//Prefab database
	[SerializeField] PrefabDatabase prefabDB;
	//Map size
	[SerializeField] int mazeX;
    [SerializeField] int mazeY;

    //Map camera
    [SerializeField] Camera mapCam;

    //Prefab instances 2D array
    Maze2Cell[,] mazeCellMap;

	//List for Prim's algorithm
	List<Maze2Cell> unvisitCells = new List<Maze2Cell>();

    [SerializeField] TMP_Text jewelText;
    int totalJewels = 0;
    List<Jewel> listOfJewels = new();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Application.targetFrameRate = 60;
        GenerateMaze();
    }

    public void CollectJewel(Jewel collectedJewel)
    {
        listOfJewels.Remove(collectedJewel);
        jewelText.text = $"Jewels: {totalJewels-listOfJewels.Count} / {totalJewels}";
    }

    void GenerateMaze()
    {
        mazeCellMap = new Maze2Cell[mazeX, mazeY];
        mapCam.transform.position = new Vector3
            (prefabDB.cellPrefab.mazeSize * (mazeX - 1) / 2,
            Mathf.Max(mazeX-1.5f, mazeY-1.5f) * (prefabDB.cellPrefab.mazeSize-1),
            prefabDB.cellPrefab.mazeSize * (mazeY - 1) / 2);

        for (int x = 0; x < mazeX; x++)
        {
            for (int y = 0; y < mazeY; y++)
            {
                Maze2Cell cell = Instantiate(prefabDB.cellPrefab);
                cell.transform.position = new Vector3(cell.mazeSize * x, 0, cell.mazeSize * y);

                mazeCellMap[x, y] = cell;
                //Assign the current position to cell
                cell.Init(x, y);
            }
        }

        Maze2Cell startCell = mazeCellMap[Random.Range(2,mazeX-2), Random.Range(2,mazeY-2)];
        Player.instance.gameObject.SetActive(false);
        Player.instance.transform.position = startCell.transform.position;
        unvisitCells.Add(startCell);

		RecursiveRandomPrim(startCell);

        for (int i = 0; i<mazeCellMap.GetLength(1); i++)
        {
            mazeCellMap[0, i].gameObject.SetActive(false);
            mazeCellMap[1, i].gameObject.SetActive(false);
            mazeCellMap[mazeX-1, i].gameObject.SetActive(false);
            mazeCellMap[mazeX-2, i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mazeCellMap.GetLength(0); i++)
        {
            mazeCellMap[i, 0].gameObject.SetActive(false);
            mazeCellMap[i, 1].gameObject.SetActive(false);
            mazeCellMap[i, mazeY-1].gameObject.SetActive(false);
            mazeCellMap[i, mazeY-2].gameObject.SetActive(false);
        }

        AddStuff(startCell);
    }

    void AddStuff(Maze2Cell startCell)
    {
        List<Maze2Cell> availableCells = new();
        for (int i = 0; i < mazeCellMap.GetLength(0); i++)
        {
            for (int j = 0; j < mazeCellMap.GetLength(1); j++)
            {
                if (mazeCellMap[i, j].gameObject.activeSelf)
                    availableCells.Add(mazeCellMap[i, j]);
            }
        }
        availableCells.Remove(startCell);

        totalJewels = Mathf.Max(mazeX, mazeY)*2;
        jewelText.text = $"Jewels: {0} / {totalJewels}";

        for (int i = 0; i<totalJewels; i++) //add jewels
        {
            Maze2Cell randomCell = availableCells[Random.Range(0, availableCells.Count)];
            Jewel newJewel = Instantiate(prefabDB.jewelPrefab);
            newJewel.transform.position = new Vector3(randomCell.transform.position.x, 1.75f, randomCell.transform.position.z);
            newJewel.name = $"Jewel {i}";
            listOfJewels.Add(newJewel);

            for (int j = 0; j < 2; j++)
            {
                int randomWall = Random.Range(-1, randomCell.listOfWalls.Count);
                if (randomWall > -1)
                {
                    randomCell.listOfWalls[randomWall].SetActive(true);
                    randomCell.listOfWalls.RemoveAt(randomWall);
                }
            }

            availableCells.Remove(randomCell);
        }

        for (int i = 0; i<5; i++)
        {
            Maze2Cell randomCell = availableCells[Random.Range(0, availableCells.Count)];
            Spinner newSpinner = Instantiate(prefabDB.spinnerPrefab);
            newSpinner.transform.position = new Vector3(randomCell.transform.position.x, 1.75f, randomCell.transform.position.z);
            newSpinner.name = $"Spinner {i}";
            availableCells.Remove(randomCell);
        }

        startCell.gameObject.SetActive(true);
        Player.instance.gameObject.SetActive(true);
    }

    void RecursiveRandomPrim(Maze2Cell startCell)
    {
        unvisitCells.Remove(startCell);
        if (!startCell.isVisited)
        {
            startCell.isVisited = true;
            //This cell will not be a wall cell
            startCell.gameObject.SetActive(false);

            //Instant connect from main path, hide the wall cell in between
            if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Right)
            {
                mazeCellMap[startCell.locX - 1, startCell.locY].gameObject.SetActive(false);
            }
            else if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Left)
            {
                mazeCellMap[startCell.locX + 1, startCell.locY].gameObject.SetActive(false);
            }
            else if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Up)
            {
                mazeCellMap[startCell.locX, startCell.locY + 1].gameObject.SetActive(false);
            }
            else if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Down)
            {
                mazeCellMap[startCell.locX, startCell.locY - 1].gameObject.SetActive(false);
            }

            //Standard connection, check unvisited cells nearby start cell
            List<Maze2Cell> neighborUnvisitedCells = CheckCellSurroundings(startCell);


            if (neighborUnvisitedCells.Count > 0)
            {
                //Connect to one of the nearby unvisit cells
                Maze2Cell endCell = neighborUnvisitedCells[Random.Range(0, neighborUnvisitedCells.Count)];
                endCell.isVisited = true;
                endCell.gameObject.SetActive(false);

                if (endCell.locX < startCell.locX)
                {
                    mazeCellMap[startCell.locX - 1, startCell.locY].gameObject.SetActive(false);
                }
                else if (endCell.locX > startCell.locX)
                {
                    mazeCellMap[startCell.locX + 1, startCell.locY].gameObject.SetActive(false);
                }
                else if (endCell.locY < startCell.locY)
                {
                    mazeCellMap[startCell.locX, startCell.locY - 1].gameObject.SetActive(false);
                }
                else if (endCell.locY > startCell.locY)
                {
                    mazeCellMap[startCell.locX, startCell.locY + 1].gameObject.SetActive(false);
                }

                //Remove visited endCell from unvisited cell list
                neighborUnvisitedCells.Remove(endCell);

				//Get all unvisited cells around startCell & endCell and add to the unvisitCells list
				unvisitCells.AddRange(neighborUnvisitedCells);
                //Since end cell is also changed to visited status, add unvisited cells nearby end cell as well
                unvisitCells.AddRange(CheckCellSurroundings(endCell));

            }
        }
        if (unvisitCells.Count > 0)
        {
            //As long as there is unvisited cell in the list, keep the recursive progress
            //Randomly choose one cell and continue
            RecursiveRandomPrim(unvisitCells[Random.Range(0, unvisitCells.Count)]);
        }
    }

    List<Maze2Cell> CheckCellSurroundings(Maze2Cell cell)
    {
        List<Maze2Cell> neighborUnvisitedCells = new List<Maze2Cell>();

        //Check if surrounding cells are unvisited and add them to the list, skip over the wall cell (-2/+2)
        if (cell.locX - 2 > 0)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX - 2, cell.locY];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Left;
            }
        }
        if (cell.locX + 2 < mazeX - 1)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX + 2, cell.locY];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Right;
            }
        }
        if (cell.locY - 2 > 0)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY - 2];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Up;
            }
        }
        if (cell.locY + 2 < mazeY - 1)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY + 2];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Down;
            }
        }

        return neighborUnvisitedCells;
    }
}
