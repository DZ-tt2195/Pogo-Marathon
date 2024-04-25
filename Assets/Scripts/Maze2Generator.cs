using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class Maze2Generator : MonoBehaviour
{

#region Variables

    public static Maze2Generator instance;

    int mapX;
    int mapY;

    [SerializeField] Camera mapCam; //Map camera

    Maze2Cell[,] mazeCellMap; //Prefab instances 2D array
    List<Maze2Cell> unvisitCells = new List<Maze2Cell>(); //List for Prim's algorithm

    [SerializeField] TMP_Text jewelText;
    int totalJewels = 0;
    List<Jewel> listOfJewels = new();

    [SerializeField] GameObject gameOverObject;
    TMP_Text endText;
    TMP_Text statsText;

    public int destroyedTiles = 0;
    Stopwatch stopwatch;

    #endregion

#region Setup

    private void Awake()
    {
        if (CarryVariables.instance == null)
            SceneManager.LoadScene("Title");

        instance = this;
        endText = gameOverObject.transform.Find("End Text").GetComponent<TMP_Text>();
        statsText = gameOverObject.transform.Find("Stats Text").GetComponent<TMP_Text>();
        gameOverObject.SetActive(false);
    }

    void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();

        mapX = CarryVariables.instance.settingsToUse.levelSizeX;
        mapY = CarryVariables.instance.settingsToUse.levelSizeY;
        GenerateMaze();
    }

    void GenerateMaze()
    {
        mazeCellMap = new Maze2Cell[mapX, mapY];
        mapCam.transform.position = new Vector3
            (CarryVariables.instance.prefabDB.cellPrefab.mazeSize * (mapX - 1) / 2,
            Mathf.Max(mapX - 1.5f, mapY - 1.5f) * (CarryVariables.instance.prefabDB.cellPrefab.mazeSize - 1),
            CarryVariables.instance.prefabDB.cellPrefab.mazeSize * (mapY - 1) / 2);

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                Maze2Cell cell = Instantiate(CarryVariables.instance.prefabDB.cellPrefab);
                cell.transform.position = new Vector3(cell.mazeSize * x, 0, cell.mazeSize * y);

                mazeCellMap[x, y] = cell;
                //Assign the current position to cell
                cell.Init(x, y);
            }
        }

        Maze2Cell startCell = mazeCellMap[UnityEngine.Random.Range(2, mapX - 2), UnityEngine.Random.Range(2, mapY - 2)];
        Player.instance.gameObject.SetActive(false);
        Player.instance.transform.position = startCell.transform.position;
        unvisitCells.Add(startCell);

        RecursiveRandomPrim(startCell);

        for (int i = 0; i < mazeCellMap.GetLength(1); i++)
        {
            mazeCellMap[0, i].gameObject.SetActive(false);
            mazeCellMap[1, i].gameObject.SetActive(false);
            mazeCellMap[mapX - 1, i].gameObject.SetActive(false);
            mazeCellMap[mapX - 2, i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mazeCellMap.GetLength(0); i++)
        {
            mazeCellMap[i, 0].gameObject.SetActive(false);
            mazeCellMap[i, 1].gameObject.SetActive(false);
            mazeCellMap[i, mapY - 1].gameObject.SetActive(false);
            mazeCellMap[i, mapY - 2].gameObject.SetActive(false);
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
                {
                    availableCells.Add(mazeCellMap[i, j]);
                }
            }
        }
        availableCells.Remove(startCell);

        totalJewels = Mathf.Min(CarryVariables.instance.settingsToUse.numJewels, availableCells.Count);
        jewelText.text = $"Jewels: {0} / {totalJewels}";

        for (int i = 0; i < totalJewels; i++) //add jewels
        {
            try
            {
                Maze2Cell randomCell = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
                Jewel newJewel = Instantiate(CarryVariables.instance.prefabDB.jewelPrefab);
                newJewel.transform.position = new Vector3(randomCell.transform.position.x, 1.75f, randomCell.transform.position.z);
                newJewel.name = $"Jewel {i}";
                listOfJewels.Add(newJewel);
                availableCells.Remove(randomCell);
            }
            catch
            {
                break;
            }
        }

        for (int i = 0; i < CarryVariables.instance.settingsToUse.numFlying; i++)
        {
            try
            {
                Maze2Cell randomCell = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
                GameObject newFlying = Instantiate(CarryVariables.instance.prefabDB.flyingCubePrefab);
                newFlying.transform.position = new Vector3(randomCell.transform.position.x, 1.75f, randomCell.transform.position.z);
                newFlying.name = $"Flying Cube {i}";
                availableCells.Remove(randomCell);
            }
            catch
            {
                break;
            }
        }

        for (int i = 0; i < CarryVariables.instance.settingsToUse.numSpinners; i++)
        {
            try
            {
                Maze2Cell randomCell = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
                Spinner newSpinner = Instantiate(CarryVariables.instance.prefabDB.spinnerPrefab);
                newSpinner.transform.position = new Vector3(randomCell.transform.position.x, 1.75f, randomCell.transform.position.z);
                newSpinner.name = $"Spinner {i}";
                availableCells.Remove(randomCell);
            }
            catch
            {
                break;
            }
        }

        for (int i = 0; i < CarryVariables.instance.settingsToUse.blanked; i++)
        {
            Maze2Cell randomCell = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
            availableCells.Remove(randomCell);
            randomCell.gameObject.SetActive(false);
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
                Maze2Cell endCell = neighborUnvisitedCells[UnityEngine.Random.Range(0, neighborUnvisitedCells.Count)];
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
            RecursiveRandomPrim(unvisitCells[UnityEngine.Random.Range(0, unvisitCells.Count)]);
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
        if (cell.locX + 2 < mapX - 1)
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
        if (cell.locY + 2 < mapY - 1)
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

    #endregion

#region Gameplay

    public void CollectJewel(Jewel collectedJewel)
    {
        listOfJewels.Remove(collectedJewel);
        jewelText.text = $"Jewels: {totalJewels-listOfJewels.Count} / {totalJewels}";

        if (listOfJewels.Count == 0)
            EndGame(true);
    }

    public void EndGame(bool won)
    {
        gameOverObject.SetActive(true);
        Player.instance.currentState = Player.GameState.End;
        endText.text = (won) ? "You Won!" : "You Died";

        stopwatch.Stop();
        TimeSpan x = stopwatch.Elapsed;
        string part1 = $"Time Taken: {x.Minutes}:{(x.Seconds < 10 ? $"0{x.Seconds}" : $"{x.Seconds}")}.{x.Milliseconds}";
        statsText.text = $"{part1}\nTiles Destroyed: {destroyedTiles}";
    }

    #endregion

}
