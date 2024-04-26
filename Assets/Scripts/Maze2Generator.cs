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

    [SerializeField] TMP_Text jewelText; //text displaying number of jewels
    int totalJewels = 0;
    List<Jewel> listOfJewels = new(); //store all jewels

    [SerializeField] GameObject gameOverObject; //game over screen
    TMP_Text endText;
    TMP_Text statsText;

    public int destroyedTiles = 0;
    Stopwatch stopwatch;

    #endregion

#region Setup

    private void Awake()
    {
        if (CarryVariables.instance == null) //if carry variables object isn't around, go to title screen
            SceneManager.LoadScene("Title");

        instance = this;
        endText = gameOverObject.transform.Find("End Text").GetComponent<TMP_Text>();
        statsText = gameOverObject.transform.Find("Stats Text").GetComponent<TMP_Text>();
        gameOverObject.SetActive(false);
    }

    void Start()
    {
        stopwatch = new Stopwatch(); //start a stopwatch for amount of time spent
        stopwatch.Start();

        mapX = CarryVariables.instance.settingsToUse.levelSizeX+2; //increase numbers by +2
        mapY = CarryVariables.instance.settingsToUse.levelSizeY+2;
        GenerateMaze();
    }

    void GenerateMaze()
    {
        mazeCellMap = new Maze2Cell[mapX, mapY];
        //center the camera around the middle of the level
        mapCam.transform.position = new Vector3
            (CarryVariables.instance.prefabDB.cellPrefab.mazeSize * (mapX - 1) / 2,
            Mathf.Max(mapX - 1.5f, mapY - 1.5f) * (CarryVariables.instance.prefabDB.cellPrefab.mazeSize - 1),
            CarryVariables.instance.prefabDB.cellPrefab.mazeSize * (mapY - 1) / 2);

        for (int x = 2; x < mapX-2; x++) //offset by 2 because the outer 2 borders will always be filled and I don't want that
        {
            for (int y = 2; y < mapY-2; y++)
            {
                Maze2Cell cell = Instantiate(CarryVariables.instance.prefabDB.cellPrefab);
                cell.transform.position = new Vector3(cell.mazeSize * x, 0, cell.mazeSize * y);

                mazeCellMap[x, y] = cell;
                cell.Init(x, y); //Assign the current position to cell
            }
        }

        //choose a random starting position for the player and disable them for now
        Maze2Cell startCell = mazeCellMap[UnityEngine.Random.Range(2, mapX - 2), UnityEngine.Random.Range(2, mapY - 2)];
        Player.instance.gameObject.SetActive(false);
        Player.instance.transform.position = startCell.transform.position;
        unvisitCells.Add(startCell);

        RecursiveRandomPrim(startCell);
        AddOtherStuff(startCell); //after generating cells, add other stuff
    }

    /// <summary>
    /// add in jewels, spinners, flying cubes, and blanks
    /// </summary>
    /// <param name="startCell">player's starting position</param>
    void AddOtherStuff(Maze2Cell startCell)
    {
        List<Maze2Cell> availableCells = new(); //find all cells that are still usable
        for (int i = 0; i < mazeCellMap.GetLength(0); i++)
        {
            for (int j = 0; j < mazeCellMap.GetLength(1); j++)
            {
                try
                {
                    if (mazeCellMap[i, j].gameObject.activeSelf)
                    {
                        availableCells.Add(mazeCellMap[i, j]);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        availableCells.Remove(startCell); //remove player's starting position

        totalJewels = Mathf.Min(CarryVariables.instance.settingsToUse.numJewels, availableCells.Count); //calculate number of jewels to use
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

        for (int i = 0; i < CarryVariables.instance.settingsToUse.numFlying; i++) //add flying cubes
        {
            try
            {
                Maze2Cell randomCell = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
                GameObject newFlying = Instantiate(CarryVariables.instance.prefabDB.flyingCubePrefab);
                newFlying.transform.position = new Vector3(randomCell.transform.position.x, 1.75f, randomCell.transform.position.z);
                newFlying.name = $"Flying Cube {i}";
                availableCells.Remove(randomCell);
            }
            catch //if there aren't enough tiles left, stop
            {
                break;
            }
        }

        for (int i = 0; i < CarryVariables.instance.settingsToUse.numSpinners; i++) //add spinners
        {
            try
            {
                Maze2Cell randomCell = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
                Spinner newSpinner = Instantiate(CarryVariables.instance.prefabDB.spinnerPrefab);
                newSpinner.transform.position = new Vector3(randomCell.transform.position.x, 1.75f, randomCell.transform.position.z);
                newSpinner.name = $"Spinner {i}";
                availableCells.Remove(randomCell);
            }
            catch //if there aren't enough tiles left, stop
            {
                break;
            }
        }

        for (int i = 0; i < CarryVariables.instance.settingsToUse.blanked; i++) //remove some tiles
        {
            try
            {
                Maze2Cell randomCell = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
                availableCells.Remove(randomCell);
                randomCell.gameObject.SetActive(false);
            }
            catch //if there aren't enough tiles left, stop
            {
                break;
            }
        }

        startCell.gameObject.SetActive(true); //enable player
        Player.instance.gameObject.SetActive(true);
    }

    void RecursiveRandomPrim(Maze2Cell startCell)
    {
        unvisitCells.Remove(startCell);
        if (!startCell.isVisited)
        {
            startCell.isVisited = true;
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

    /// <summary>
    /// //Check if surrounding cells are unvisited and add them to the list, skip over the wall cell (-2/+2)
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    List<Maze2Cell> CheckCellSurroundings(Maze2Cell cell)
    {
        List<Maze2Cell> neighborUnvisitedCells = new List<Maze2Cell>();

        try
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX - 2, cell.locY];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Left;
            }
        } catch { }

        try
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX + 2, cell.locY];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Right;
            }
        } catch {}

        try
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY - 2];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Up;
            }
        } catch {}

        try
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY + 2];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Down;
            }
        } catch { }

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
